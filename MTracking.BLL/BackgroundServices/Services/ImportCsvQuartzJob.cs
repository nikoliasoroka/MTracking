using AutoMapper.Internal;
using CsvHelper;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using MTracking.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MTracking.BLL.BackgroundServices.Mappings;
using MTracking.BLL.BackgroundServices.Models;
using MTracking.BLL.DTOs.Comparing;
using MTracking.BLL.DTOs.Comparing.Enums;
using MTracking.Core.Entities;
using MTracking.Core.Enums;
using MTracking.Core.Logger;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;
using File = MTracking.Core.Entities.File;

namespace MTracking.BLL.BackgroundServices.Services
{
    [DisallowConcurrentExecution]
    public class ImportCsvQuartzJob : IJob
    {
        private readonly IConfigurationSection _csvConfiguration;
        private readonly IServiceProvider _provider;

        private IUnitOfWork _unitOfWork;
        private IRepository<User> _userRepository;
        private IRepository<Core.Entities.File> _fileRepository;
        private IRepository<TimeLog> _timeLogRepository;
        private IRepository<Topic> _topicRepository;
        private IRepository<Description> _descriptionRepository;
        private IRepository<ExportTimeLog> _exportTimeLogRepository;
        private IRepository<Import> _importRepository;
        private IRepository<Reminder> _reminderRepository;


        public ImportCsvQuartzJob(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _csvConfiguration = configuration.GetSection("ImportCSV");
            _provider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Delay(7000);

            var importFolder = _csvConfiguration["ImportFolder"];
            var exportFolder = _csvConfiguration["ExportFolder"];

            if (Directory.Exists(exportFolder))
            {
                await ProcessEmployeesCsv(exportFolder);
                await ProcessFilesCsv(exportFolder);
                await ProcessBillingCodesCsv(exportFolder);
                await ProcessHoursBillingCsv(exportFolder);
            }
            else
                Logger.Error($"The directory {exportFolder} doesn't exist!");

            if (Directory.Exists(importFolder))
                await InsertHours(importFolder);
            else
                Logger.Error($"The directory {importFolder} doesn't exist!");
        }

        #region import CommitEmployees file

        private async Task ProcessEmployeesCsv(string path)
        {
            try
            {
                using var scope = _provider.CreateScope();
                _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                _userRepository = _unitOfWork.GetRepository<User>();
                _importRepository = _unitOfWork.GetRepository<Import>();
                _reminderRepository = _unitOfWork.GetRepository<Reminder>();
                _timeLogRepository = _unitOfWork.GetRepository<TimeLog>();

                var directoryFiles = Directory.GetFiles(path, "CommitEmployees*.csv")
                    .OrderBy(Directory.GetCreationTime)
                    .ToList();

                if (directoryFiles.Count == 0)
                    return;

                var dbImportFiles = await _importRepository.Table
                    .Where(x => x.FileType == ImportFileType.CommitEmployees)
                    .Select(x => x.FileName)
                    .ToListAsync();

                var file = directoryFiles.Where(x => dbImportFiles.All(z => z != Path.GetFileName(x))).TakeLast(1)
                    .FirstOrDefault();

                if (file is null)
                    return;

                try
                {
                    //using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    using var reader = new StreamReader(file);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csv.Configuration.RegisterClassMap<EmployeeMapping>();
                    csv.Configuration.BadDataFound = null;

                    var employeesCsv = csv.GetRecords<EmployeeCsv>().ToList();

                    var mappedUsers = employeesCsv.Select(x => new User
                    {
                        CommitId = string.IsNullOrWhiteSpace(x.CommitId) ? 0 : Convert.ToInt32(x.CommitId.OnlyDigits()),
                        HebrewName = string.IsNullOrWhiteSpace(x.HebrewName)
                            ? null
                            : x.HebrewName.RemoveQuotes().ReplaceFromSymbols(),
                        EnglishName = string.IsNullOrWhiteSpace(x.EnglishName)
                            ? null
                            : x.EnglishName.RemoveQuotes().ReplaceFromSymbols(),
                        UserName = string.IsNullOrWhiteSpace(x.UserName)
                            ? null
                            : x.UserName.RemoveQuotes().ReplaceFromSymbols(),
                        EmployeeRunCommit = x.EmployeeSoftwareInvention.RemoveQuotes() == "Y",
                        EmployeeSoftwareInvention = x.EmployeeSoftwareInvention.RemoveQuotes() == "Y",
                        Email = string.IsNullOrWhiteSpace(x.Email)
                            ? null
                            : x.Email.Trim().RemoveQuotes().ReplaceFromSymbols().ToLower()
                    }).ToList();

                    var employeesDb = await _userRepository.Table
                        //.Include(x => x.Reminders)
                        //.Include(x => x.TimeLogs)
                        .ToListAsync();

                    var employeesToSoftDelete = employeesDb.AsParallel()
                        .Where(x => mappedUsers.Where(u => u.UserName == null || !u.EmployeeRunCommit)
                            .Select(c => c.CommitId).Contains(x.CommitId))
                        .ToList();

                    var employeesToDelete = employeesDb.AsParallel()
                        .Where(x => !mappedUsers.Select(c => c.CommitId).Contains(x.CommitId))
                        .ToList();

                    employeesToSoftDelete.AddRange(employeesToDelete);
                    await SoftDeleteEmployees(employeesToSoftDelete);
                    //await DeleteEmployees(employeesToDelete);

                    var employeesToInsert = mappedUsers.Where(x => x.UserName != null && x.EmployeeRunCommit)
                        .AsParallel()
                        .Where(x => !employeesDb.Select(z => z.CommitId).Contains(x.CommitId))
                        .ToList();

                    await InsertEmployees(employeesToInsert);

                    var updatedEmployeesDb = await _userRepository.Table.ToListAsync();

                    var employeesToUpdate = updatedEmployeesDb.AsParallel()
                        .Where(x => mappedUsers.Where(u => u.UserName != null && u.EmployeeRunCommit)
                            .Select(c => c.CommitId).Contains(x.CommitId))
                        .ToList();

                    await UpdateEmployees(employeesToUpdate, mappedUsers.Where(x => x.UserName != null).ToList());

                    stopWatch.Stop();

                    await _importRepository.InsertAsync(new Import
                    {
                        FileType = ImportFileType.CommitEmployees,
                        FileName = Path.GetFileName(file),
                        InsertedRecords = employeesToInsert.Count,
                        UpdatedRecords = employeesToUpdate.Count,
                        Performance = Math.Round(stopWatch.Elapsed.TotalSeconds, 2),
                    });

                    await _unitOfWork.SaveAsync();

                    //transactionScope.Complete();
                }
                catch (Exception exception)
                {
                    Logger.Fatal($"Can't parse csv file {Path.GetFileName(file)}", exception);
                }
            }
            catch (Exception exception)
            {
                Logger.Fatal($"Can't parse csv CommitEmployees files", exception);
            }
        }

        private async Task InsertEmployees(IEnumerable<User> employeesToInsert)
        {
            const string defaultPassword = "12345678";

            CreatePasswordHash(defaultPassword, out var passwordHash, out var passwordSalt);

            var users = employeesToInsert.Select(x =>
            {
                x.PasswordSalt = passwordSalt;
                x.PasswordHash = passwordHash;
                x.CreatedOn = DateTime.UtcNow;
                x.IsPasswordChanged = false;
                return x;
            }).ToList();

            await _userRepository.InsertRangeAsync(users);
            await _unitOfWork.SaveAsync();
        }

        private async Task UpdateEmployees(IEnumerable<User> employeesToUpdate, IReadOnlyCollection<User> mappedUsers)
        {
            var users = employeesToUpdate.AsParallel().Select(x =>
            {
                var mappedUser = mappedUsers.AsParallel().FirstOrDefault(z => z.CommitId == x.CommitId);

                if (mappedUser != null)
                {
                    x.CommitId = mappedUser.CommitId;
                    x.HebrewName = mappedUser.HebrewName;
                    x.EnglishName = mappedUser.EnglishName;
                    x.UserName = mappedUser.UserName;
                    x.EmployeeRunCommit = mappedUser.EmployeeRunCommit;
                    x.EmployeeSoftwareInvention = mappedUser.EmployeeSoftwareInvention;
                    x.UpdatedOn = DateTime.UtcNow;
                    x.Email = mappedUser.Email;
                }

                return x;
            }).ToList();

            await _userRepository.UpdateRangeAsync(users);
            await _unitOfWork.SaveAsync();
        }

        private async Task SoftDeleteEmployees(List<User> employeesToDelete)
        {
            employeesToDelete.ForEach(user =>
            {
                user.UserName = user.CommitId.ToString();
                user.EmployeeRunCommit = false;
            });

            await _userRepository.UpdateRangeAsync(employeesToDelete);
            await _unitOfWork.SaveAsync();
        }

        private async Task DeleteEmployees(List<User> employeesToDelete)
        {
            try
            {
                foreach (var user in employeesToDelete)
                {
                    var reminders = user.Reminders.Select(x => x.Id).ToList();
                    var timeLogs = user.TimeLogs.Select(x => x.Id).ToList();
                    foreach (var reminder in reminders)
                    {
                        await _reminderRepository.DeleteAsync(reminder);
                    }

                    foreach (var timeLog in timeLogs)
                    {
                        await _timeLogRepository.DeleteAsync(timeLog);
                    }

                    await _userRepository.DeleteAsync(user.Id);
                }

                await _unitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        #endregion

        #region import CommitFiles file

        private async Task ProcessFilesCsv(string path)
        {
            try
            {
                using var scope = _provider.CreateScope();
                _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                _fileRepository = _unitOfWork.GetRepository<Core.Entities.File>();
                _importRepository = _unitOfWork.GetRepository<Import>();

                var directoryFiles = Directory.GetFiles(path, "CommitFiles*.csv")
                    .OrderBy(Directory.GetCreationTime)
                    .ToList();

                if (directoryFiles.Count == 0)
                    return;

                var dbImportFiles = await _importRepository.Table
                    .Where(x => x.FileType == ImportFileType.CommitFiles)
                    .Select(x => x.FileName)
                    .ToListAsync();

                var file = directoryFiles.Where(x => dbImportFiles.All(z => z != Path.GetFileName(x))).TakeLast(1)
                    .FirstOrDefault();
                var oldFile = directoryFiles.FirstOrDefault(x =>
                    x.Contains(dbImportFiles.TakeLast(1).FirstOrDefault() ?? "false"));

                if (file is null)
                    return;

                try
                {
                    //using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    using var reader = new StreamReader(file);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csv.Configuration.RegisterClassMap<FileMapping>();
                    csv.Configuration.BadDataFound = null;

                    var filesCsv = csv.GetRecords<FileCsv>().ToList();

                    var mappedFiles = filesCsv.Select(x => new Core.Entities.File
                        {
                            Name =
                                $"{x.PortfolioNumber.RemoveQuotes().ReplaceFromSymbols()} {x.CaseName.RemoveQuotes().ReplaceFromSymbols()}",
                            CaseName = x.CaseName.RemoveQuotes(),
                            EnglishCaseName =
                                $"{x.PortfolioNumber.RemoveQuotes()} {x.EnglishCaseName.RemoveQuotes().ReplaceFromSymbols()}",
                            PortfolioNumber = x.PortfolioNumber.RemoveQuotes().СlearToDigitsAndPunctuations(),
                            PortfolioStatus = x.PortfolioStatus.RemoveQuotes() == "סגור"
                                ? CommitFileStatus.Closed
                                : CommitFileStatus.Active,
                            OpeningOn = DateTime.TryParseExact(x.OpeningOn, "MM-dd-yyyy", null, DateTimeStyles.None,
                                out var openingOn)
                                ? openingOn
                                : (DateTime?) null,
                            ClosingOn = DateTime.TryParseExact(x.ClosingOn, "MM-dd-yyyy", null, DateTimeStyles.None,
                                out var closingOn)
                                ? closingOn
                                : (DateTime?) null,
                            IsChargedCase = x.IsChargedCase.RemoveQuotes() == "Y"
                        })
                        .GroupBy(x => x.PortfolioNumber)
                        .Select(x => x.First())
                        .ToDictionary(key => key.PortfolioNumber);

                    var freshData = await CompareFiles(oldFile, file, ImportFileType.CommitFiles);
                    var dictFreshData = freshData.ToDictionary(key => key.PortfolioNumber);

                    var freshParsedFiles = mappedFiles.Where(x => x.Key != null && dictFreshData.Keys.Contains(x.Key))
                        .ToDictionary(key => key.Value.PortfolioNumber, value => value.Value);

                    var filesToInsert = new List<Core.Entities.File>();
                    var filesToUpdate = new Dictionary<string, Core.Entities.File>();

                    if (freshParsedFiles.Any())
                    {
                        var filesDb = await _fileRepository.Table
                            .ToDictionaryAsync(key => key.PortfolioNumber.СlearToDigitsAndPunctuations());

                        filesToInsert = freshParsedFiles
                            .Where(x => !filesDb.Keys.Contains(x.Key))
                            .Select(x => x.Value)
                            .ToList();

                        await InsertFiles(filesToInsert);

                        filesToUpdate = filesDb
                            .Where(x => freshParsedFiles.Keys.Contains(x.Key))
                            .ToDictionary(key => key.Key, value => value.Value);

                        await UpdateFile(filesToUpdate, freshParsedFiles);
                    }

                    stopWatch.Stop();

                    await _importRepository.InsertAsync(new Import
                    {
                        FileType = ImportFileType.CommitFiles,
                        FileName = Path.GetFileName(file),
                        InsertedRecords = filesToInsert.Count,
                        UpdatedRecords = filesToUpdate.Count,
                        Performance = Math.Round(stopWatch.Elapsed.TotalSeconds, 2)
                    });

                    await _unitOfWork.SaveAsync();

                    //transactionScope.Complete();
                }
                catch (Exception exception)
                {
                    Logger.Fatal($"Can't parse csv file {Path.GetFileName(file)}", exception);
                }
            }
            catch (Exception exception)
            {
                Logger.Fatal($"Can't parse csv CommitFiles files", exception);
            }
        }

        private async Task InsertFiles(IReadOnlyCollection<Core.Entities.File> filesToInsert)
        {
            filesToInsert.ForAll(x => x.CreatedOn = DateTime.UtcNow);

            await _fileRepository.InsertRangeAsync(filesToInsert);
            await _unitOfWork.SaveAsync();
        }

        private async Task UpdateFile(IDictionary<string, Core.Entities.File> filesToUpdate, IDictionary<string, Core.Entities.File> mappedFiles)
        {
            var files = filesToUpdate.Select(fileToUpdate =>
            {
                var (key, value) = fileToUpdate;

                if (mappedFiles.TryGetValue(key, out var file))
                {
                    value.Name = file.Name;
                    value.CaseName = file.CaseName;
                    value.EnglishCaseName = file.EnglishCaseName;
                    value.PortfolioNumber = file.PortfolioNumber;
                    value.PortfolioStatus = file.PortfolioStatus;
                    value.OpeningOn = file.OpeningOn;
                    value.ClosingOn = file.ClosingOn;
                    value.IsChargedCase = file.IsChargedCase;
                    value.UpdatedOn = DateTime.UtcNow;

                    return value;
                }

                return value;
            }).ToList();

            await _fileRepository.UpdateRangeAsync(files);
            await _unitOfWork.SaveAsync();
        }

        #endregion

        #region import CommitBillingCode file

        private async Task ProcessBillingCodesCsv(string path)
        {
            try
            {
                using var scope = _provider.CreateScope();
                _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                _topicRepository = _unitOfWork.GetRepository<Topic>();
                _importRepository = _unitOfWork.GetRepository<Import>();

                var directoryFiles = Directory.GetFiles(path, "CommitBillingCodes*.csv")
                    .OrderBy(Directory.GetCreationTime)
                    .ToList();

                if (directoryFiles.Count == 0)
                    return;

                var dbImportFiles = await _importRepository.Table
                    .Where(x => x.FileType == ImportFileType.CommitBillingCode)
                    .Select(x => x.FileName)
                    .ToListAsync();

                var file = directoryFiles.Where(x => dbImportFiles.All(z => z != Path.GetFileName(x))).TakeLast(1)
                    .FirstOrDefault();

                if (file is null)
                    return;

                try
                {
                    //using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    using var reader = new StreamReader(file);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csv.Configuration.RegisterClassMap<BillingCodeMapping>();
                    csv.Configuration.BadDataFound = null;

                    var topicsCsv = csv.GetRecords<BillingCodeCsv>().ToList();

                    var mappedTopics = topicsCsv.Select(x => new Topic
                    {
                        BillingCodeId = Convert.ToInt32(x.BillingCodeId.OnlyDigits()),
                        Name = x.Name.RemoveQuotes().ReplaceFromSymbols(),
                        Detail = x.Detail.RemoveQuotes().ReplaceFromSymbols(),
                        UnitType = (BillingCodeUnitType) Convert.ToInt32(x.UnitType.OnlyDigits()),
                        IsCustom = false,
                        IsNotary = Convert.ToBoolean(x.IsNotary.RemoveQuotes()),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    }).ToList();

                    var dbTopics = await _topicRepository.Table
                        .Where(x => !x.IsCustom)
                        .ToListAsync();

                    var topicsToInsert = mappedTopics
                        .Where(x => dbTopics.All(z => z.BillingCodeId != x.BillingCodeId))
                        .ToList();

                    await InsertTopics(topicsToInsert);

                    var topicsToUpdate = dbTopics
                        .Where(x => mappedTopics.Any(z => z.BillingCodeId == x.BillingCodeId))
                        .ToList();

                    await UpdateTopics(topicsToUpdate, mappedTopics);

                    stopWatch.Stop();

                    await _importRepository.InsertAsync(new Import
                    {
                        FileType = ImportFileType.CommitBillingCode,
                        FileName = Path.GetFileName(file),
                        InsertedRecords = topicsToInsert.Count,
                        UpdatedRecords = topicsToUpdate.Count,
                        Performance = Math.Round(stopWatch.Elapsed.TotalSeconds, 2)
                    });

                    await _unitOfWork.SaveAsync();

                    //transactionScope.Complete();
                }
                catch (Exception exception)
                {
                    Logger.Fatal($"Can't parse csv file {Path.GetFileName(file)}", exception);
                }
            }
            catch (Exception exception)
            {
                Logger.Fatal($"Can't parse csv CommitBillingCode files", exception);
            }
        }

        private async Task InsertTopics(IEnumerable<Topic> topicsToInsert)
        {
            var topics = topicsToInsert.AsParallel().Select(x =>
            {
                x.CreatedOn = DateTime.UtcNow;
                return x;
            }).ToList();

            await _topicRepository.InsertRangeAsync(topics);
            await _unitOfWork.SaveAsync();
        }

        private async Task UpdateTopics(IEnumerable<Topic> topicsToUpdate, IReadOnlyCollection<Topic> mappedTopics)
        {
            var topics = topicsToUpdate.AsParallel().Select(topicToUpdate =>
            {
                var mappedTopic = mappedTopics.AsParallel()
                    .FirstOrDefault(z => z.BillingCodeId == topicToUpdate.BillingCodeId);

                if (mappedTopic != null)
                {
                    topicToUpdate.Name = mappedTopic.Name;
                    topicToUpdate.Detail = mappedTopic.Detail;
                    topicToUpdate.IsNotary = mappedTopic.IsNotary;
                    topicToUpdate.UnitType = mappedTopic.UnitType;
                    topicToUpdate.UpdatedOn = DateTime.UtcNow;
                }

                return topicToUpdate;
            }).ToList();

            await _topicRepository.UpdateRangeAsync(topics);
            await _unitOfWork.SaveAsync();
        }

        #endregion

        #region import CommitHoursBilling file

        private async Task ProcessHoursBillingCsv(string path)
        {
            try
            {
                using var scope = _provider.CreateScope();
                _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                _timeLogRepository = _unitOfWork.GetRepository<TimeLog>();
                _fileRepository = _unitOfWork.GetRepository<Core.Entities.File>();
                _userRepository = _unitOfWork.GetRepository<User>();
                _topicRepository = _unitOfWork.GetRepository<Topic>();
                _descriptionRepository = _unitOfWork.GetRepository<Description>();
                _importRepository = _unitOfWork.GetRepository<Import>();

                var directoryFiles = Directory.GetFiles(path, "CommitHoursBilling*.csv")
                    .OrderBy(Directory.GetCreationTime)
                    .ToList();

                if (directoryFiles.Count == 0)
                    return;

                var dbImportFiles = await _importRepository.Table
                    .Where(x => x.FileType == ImportFileType.CommitHoursBilling)
                    .Select(x => x.FileName)
                    .ToListAsync();

                var file = directoryFiles.Where(x => dbImportFiles.All(z => z != Path.GetFileName(x))).TakeLast(1)
                    .FirstOrDefault();
                var oldFile = directoryFiles
                    .Where(x => x.Contains(dbImportFiles.TakeLast(1).FirstOrDefault() ?? "false")).FirstOrDefault();

                if (file is null)
                    return;

                try
                {
                    //using var transactionScope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.MaxValue, TransactionScopeAsyncFlowOption.Enabled);
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    using var reader = new StreamReader(file);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csv.Configuration.RegisterClassMap<HoursBillingMapping>();
                    csv.Configuration.BadDataFound = null;

                    var hoursBillingCsv = csv.GetRecords<HoursBillingCsv>().ToList();

                    var parsedHoursBillings = hoursBillingCsv.Select(x => new HoursBillingParsed
                        {
                            CommitRecordId = string.IsNullOrWhiteSpace(x.CommitRecordId)
                                ? (int?) null
                                : Convert.ToInt32(x.CommitRecordId.OnlyDigits()),
                            BillingStatus = string.IsNullOrWhiteSpace(x.BillingStatus)
                                ? (int?) null
                                : Convert.ToInt32(x.BillingStatus.OnlyDigits()),
                            IsCharged = x.IsCharged.RemoveQuotes() == "TRUE",
                            CommitEmployeeId = Convert.ToInt32(x.CommitEmployeeId.OnlyDigits()),
                            BillingCreatedOn = DateTime.TryParseExact(x.BillingCreatedOn.RemoveQuotes(), "MM-dd-yyyy",
                                null, DateTimeStyles.None, out var createdOn)
                                ? createdOn
                                : (DateTime?) null,
                            FilePortfolioNumber = x.FilePortfolioNumber.RemoveQuotes().СlearToDigitsAndPunctuations(),
                            BillingCodeTopic = string.IsNullOrWhiteSpace(x.BillingCodeTopic)
                                ? null
                                : x.BillingCodeTopic.RemoveQuotes(),
                            BillingType = Convert.ToInt32(x.BillingType.OnlyDigits()),
                            IsNotary = x.IsNotary.RemoveQuotes() == "TRUE",
                            WorkTime = (int) TimeSpan
                                .FromHours(
                                    Convert.ToDouble(x.WorkingHours.RemoveQuotes(), CultureInfo.InvariantCulture))
                                .TotalMinutes,
                            BillingDetails = string.IsNullOrWhiteSpace(x.BillingDetails)
                                ? null
                                : x.BillingDetails.RemoveQuotes().ReplaceFromSymbols(),
                            BillingUpdatedOn = DateTime.TryParseExact(x.BillingUpdatedOn.RemoveQuotes(), "MM-dd-yyyy",
                                null, DateTimeStyles.None, out var updatedOn)
                                ? updatedOn
                                : (DateTime?) null,
                            BillingDate = DateTime.TryParseExact(x.BillingDate.RemoveQuotes(), "MM-dd-yyyy", null,
                                DateTimeStyles.None, out var date)
                                ? date
                                : (DateTime?) null,
                            InternalId = string.IsNullOrWhiteSpace(x.InternalId)
                                ? (int?) null
                                : Convert.ToInt32(x.InternalId.OnlyDigits())
                        })
                        .Where(x => x.CommitRecordId != null)
                        .ToDictionary(key => key.CommitRecordId);

                    var freshData = await CompareFiles(oldFile, file, ImportFileType.CommitHoursBilling);
                    var dictFreshData = freshData.ToDictionary(key => key.Id);

                    var freshParsedHoursBillings = parsedHoursBillings
                        .Where(x => x.Key.HasValue && dictFreshData.ContainsKey(x.Key))
                        .ToDictionary(key => key.Key, value => value.Value);

                    var timeLogToInsert = new List<TimeLog>();
                    var timeLogsByIdToUpdate = new List<TimeLog>();
                    var timeLogsByCommitIdToUpdate = new List<TimeLog>();

                    if (freshParsedHoursBillings.Any())
                    {
                        #region Description operations

                        var dbEmployeeByIds = await _userRepository.Table
                            .Select(x => new {x.Id, x.CommitId})
                            .ToDictionaryAsync(key => key.Id);

                        var dbEmployeeByCommitIds =
                            dbEmployeeByIds.ToDictionary(key => key.Value.CommitId, value => value.Value.Id);

                        var csvDescription = freshParsedHoursBillings
                            .Select(x => new
                            {
                                x.Value.BillingDetails, x.Value.CommitEmployeeId, x.Value.CommitRecordId,
                                x.Value.InternalId
                            })
                            .ToDictionary(key => key.CommitRecordId);

                        var dbDescriptions = await _descriptionRepository.Table
                            .Where(x => !x.IsCustom && x.CommitRecordId != null)
                            .Select(x => new {x.Id, x.CommitRecordId})
                            .ToDictionaryAsync(key => key.Id, value => value.CommitRecordId);

                        var descriptionsToInsert = csvDescription
                            .Where(x => x.Key.HasValue
                                        && !x.Value.InternalId.HasValue
                                        && !dbDescriptions.Values.Contains(x.Key))
                            .Select(x => new Description
                            {
                                Name = x.Value.BillingDetails,
                                IsCustom = false,
                                CommitRecordId = x.Value.CommitRecordId,
                                UserId = dbEmployeeByCommitIds.TryGetValue(x.Value.CommitEmployeeId ?? 0,
                                    out var userId)
                                    ? userId
                                    : (int?) null,
                                CreatedOn = DateTime.UtcNow
                            })
                            .Where(x => x.UserId != 0 && x.UserId != null && x.Name != null)
                            .ToList();

                        if (descriptionsToInsert.Any())
                        {
                            await _descriptionRepository.InsertRangeAsync(descriptionsToInsert);
                            await _unitOfWork.SaveAsync();
                        }

                        #endregion

                        var dbFilePortfolioNumbers = await _fileRepository.Table
                            .Select(x => new {x.Id, PortfolioNumber = x.PortfolioNumber.СlearToDigitsAndPunctuations()})
                            .ToDictionaryAsync(key => key.PortfolioNumber, value => value.Id);

                        var dbTimeLogsById = await _timeLogRepository.Table
                            .ToDictionaryAsync(key => key.Id);

                        var dbTimeLogsByCommitId = dbTimeLogsById
                            .Where(x => x.Value.CommitRecordId.HasValue)
                            .ToDictionary(key => key.Value.CommitRecordId, value => value.Value);

                        var dbTopics = await _topicRepository.Table
                            .Select(x => new {x.Id, x.Name})
                            .ToDictionaryAsync(key => key.Id);

                        var dbUpdatedDescriptions1 = await _descriptionRepository.Table
                            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                            .Select(x => new {x.Id, x.Name})
                            .ToListAsync();

                        var dbUpdatedDescriptions = dbUpdatedDescriptions1
                            .GroupBy(x => x.Name)
                            .Select(x => x.First())
                            .ToDictionary(key => key.Name, value => value.Id);

                        var validTimeLogsToInsert = freshParsedHoursBillings
                            .Where(x =>
                                !dbTimeLogsByCommitId.ContainsKey(x.Value.CommitRecordId ?? 0)
                                && !dbTimeLogsById.ContainsKey(x.Value.InternalId ?? 0)
                                && dbFilePortfolioNumbers.ContainsKey(x.Value.FilePortfolioNumber)
                                && dbEmployeeByCommitIds.ContainsKey(x.Value.CommitEmployeeId ?? 0))
                            .ToDictionary(key => key.Key, value => value.Value);

                        if (validTimeLogsToInsert.Any())
                        {
                            timeLogToInsert = validTimeLogsToInsert
                                .Select(x => new TimeLog
                                {
                                    CommitRecordId = x.Key,
                                    Date = x.Value.BillingCreatedOn ?? DateTime.Now,
                                    WorkTime = x.Value.WorkTime,
                                    BillingDateCreation = x.Value.BillingCreatedOn,
                                    ForExport = false,
                                    isSynchronize = true,
                                    BillingStatus = x.Value.BillingStatus == null
                                        ? (CommitBillingStatus?) null
                                        : (CommitBillingStatus) x.Value.BillingStatus,
                                    FileId = dbFilePortfolioNumbers.TryGetValue(x.Value.FilePortfolioNumber,
                                        out var fileId)
                                        ? fileId
                                        : (int?) null,
                                    UserId = dbEmployeeByCommitIds.TryGetValue(x.Value.CommitEmployeeId ?? 0,
                                        out var userId)
                                        ? userId
                                        : (int?) null,
                                    TopicId = dbTopics.Values.FirstOrDefault(z => z.Name == x.Value.BillingCodeTopic)
                                        ?.Id,
                                    DescriptionId =
                                        dbUpdatedDescriptions.TryGetValue(x.Value.BillingDetails ?? "",
                                            out var descriptionId)
                                            ? descriptionId
                                            : (int?) null,
                                    CreatedOn = DateTime.UtcNow
                                }).ToList();

                            await _timeLogRepository.InsertRangeAsync(timeLogToInsert);
                            await _unitOfWork.SaveAsync();
                        }

                        dbTimeLogsByCommitId = await _timeLogRepository.Table
                            .Where(x => x.CommitRecordId.HasValue)
                            .ToDictionaryAsync(key => key.CommitRecordId);

                        var timeLogsToUpdateByCommitId = dbTimeLogsByCommitId
                            .Where(x => freshParsedHoursBillings.ContainsKey(x.Key ?? 0))
                            .ToDictionary(key => key.Key, value => value.Value);

                        if (timeLogsToUpdateByCommitId.Any())
                        {
                            timeLogsByCommitIdToUpdate = timeLogsToUpdateByCommitId.Select(timeLog =>
                            {
                                var (key, value) = timeLog;

                                if (freshParsedHoursBillings.TryGetValue(key, out var result))
                                {
                                    value.WorkTime = result.WorkTime;
                                    value.BillingDateCreation = result.BillingCreatedOn;
                                    value.ForExport = false;
                                    value.isSynchronize = true;
                                    value.DescriptionId =
                                        dbUpdatedDescriptions.TryGetValue(result.BillingDetails ?? "",
                                            out var descriptionId)
                                            ? descriptionId
                                            : (int?) null;
                                    value.FileId =
                                        dbFilePortfolioNumbers.TryGetValue(result.FilePortfolioNumber, out var fileId)
                                            ? fileId
                                            : (int?) null;
                                    value.TopicId = dbTopics.Values
                                        .FirstOrDefault(z => z.Name == result.BillingCodeTopic)?.Id;
                                    value.UpdatedOn = DateTime.UtcNow;

                                    return value;
                                }

                                return value;
                            }).ToList();

                            await _timeLogRepository.UpdateRangeAsync(timeLogsByCommitIdToUpdate);
                            await _unitOfWork.SaveAsync();
                        }

                        dbTimeLogsById = await _timeLogRepository.Table
                            .ToDictionaryAsync(key => key.Id);

                        var timeLogsToUpdateById = dbTimeLogsById
                            .Where(x => !x.Value.CommitRecordId.HasValue
                                        && freshParsedHoursBillings.Values.Select(z => z.InternalId).ToList()
                                            .Contains(x.Key))
                            .ToDictionary(key => key.Key, value => value.Value);

                        if (timeLogsToUpdateById.Any())
                        {
                            timeLogsByIdToUpdate = timeLogsToUpdateById
                                .Select(timeLog =>
                                {
                                    var (key, value) = timeLog;
                                    var result =
                                        freshParsedHoursBillings.Values.FirstOrDefault(x => x.InternalId == key);
                                    if (result != null)
                                    {
                                        value.CommitRecordId = result.CommitRecordId;
                                        value.WorkTime = result.WorkTime;
                                        value.Date = result.BillingCreatedOn;
                                        value.BillingDateCreation = result.BillingCreatedOn;
                                        value.ForExport = false;
                                        value.isSynchronize = true;
                                        value.DescriptionId =
                                            dbUpdatedDescriptions.TryGetValue(result.BillingDetails ?? "",
                                                out var descriptionId)
                                                ? descriptionId
                                                : (int?) null;
                                        value.FileId =
                                            dbFilePortfolioNumbers.TryGetValue(result.FilePortfolioNumber,
                                                out var fileId)
                                                ? fileId
                                                : (int?) null;
                                        value.TopicId = dbTopics.Values
                                            .FirstOrDefault(z => z.Name == result.BillingCodeTopic)?.Id;
                                        value.UpdatedOn = DateTime.UtcNow;

                                        return value;
                                    }

                                    return value;
                                }).ToList();

                            await _timeLogRepository.UpdateRangeAsync(timeLogsByIdToUpdate);
                            await _unitOfWork.SaveAsync();
                        }
                    }

                    stopWatch.Stop();

                    await _importRepository.InsertAsync(new Import
                    {
                        FileType = ImportFileType.CommitHoursBilling,
                        FileName = Path.GetFileName(file),
                        InsertedRecords = timeLogToInsert.Count,
                        UpdatedRecords = timeLogsByCommitIdToUpdate.Count + timeLogsByIdToUpdate.Count,
                        Performance = Math.Round(stopWatch.Elapsed.TotalSeconds, 2)
                    });

                    await _unitOfWork.SaveAsync();

                    //transactionScope.Complete();
                }
                catch (Exception exception)
                {
                    Logger.Fatal($"Can't parse csv file {Path.GetFileName(file)}", exception);
                }
            }
            catch (Exception exception)
            {
                Logger.Fatal($"Can't parse csv CommitHoursBilling files", exception);
            }
        }

        #endregion

        #region export InsertHours file

        private async Task InsertHours(string path)
        {
            try
            {
                using var scope = _provider.CreateScope();
                _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                _timeLogRepository = _unitOfWork.GetRepository<TimeLog>();
                _exportTimeLogRepository = _unitOfWork.GetRepository<ExportTimeLog>();

                var timeLogs = await _timeLogRepository.Table
                    .Where(x => x.ForExport && x.User.CommitId > 0)
                    .Select(x => new
                    {
                        CommitPortfolioNumber = x.File.PortfolioNumber,
                        TimeLogId = x.Id,
                        BillingDate = x.Date,
                        BillingDetail = x.Description.Name,
                        WorkTime = x.WorkTime,
                        isSynchronize = x.isSynchronize,
                        BillingCode = x.Topic.BillingCodeId,
                        BillingType = x.Topic.UnitType,
                        CommitEmployeeId = x.User.CommitId,
                        CommitRecordId = x.CommitRecordId,
                        Notary = x.Topic.IsNotary
                    })
                    .ToListAsync();

                var fileNumber = await _exportTimeLogRepository.Table.CountAsync() + 1;

                var fileName = $"{fileNumber:0000000}CommitInsertHours.csv";

                var insertHours = timeLogs.Select(x => new InsertHoursCsv
                {
                    CommitPortfolioNumber = x.CommitPortfolioNumber,
                    TimeLogId = x.TimeLogId.ToString(),
                    BillingDateYear = x.BillingDate?.ToString("yyyy"),
                    BillingDateMonth = x.BillingDate?.ToString("MM"),
                    BillingDateDay = x.BillingDate?.ToString("dd"),
                    BillingDetail = x.BillingDetail.ReplaceToSymbols(),
                    BillingDetailSecond = string.Empty,
                    BillingType = 1.ToString(), //Convert.ToInt32(x.BillingType),
                    BillingCode = x.BillingCode.ToString(),
                    WorkTime = TimeSpan.FromMinutes(x.WorkTime).TotalHours.ToString("F2", CultureInfo.InvariantCulture)
                        .ToString(),
                    CommitEmployeeId = x.CommitEmployeeId.ToString(),
                    CommitRecordId = x.CommitRecordId.ToString(),
                    Notary = x.Notary.ToString().ToUpper()
                });

                await using var writer = new StreamWriter(Path.Combine(path, fileName));
                await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.Configuration.RegisterClassMap<InsertHoursMapping>();
                csv.Configuration.ShouldQuote = (field, context) => true;

                await csv.WriteRecordsAsync(insertHours);

                var exportedTimeLogs = await _timeLogRepository.Table
                    .Where(x => x.ForExport)
                    .ToListAsync();

                foreach (var timeLog in exportedTimeLogs)
                {
                    timeLog.ForExport = false;
                    timeLog.isSynchronize = true;
                    await _timeLogRepository.UpdateAsync(timeLog);
                }

                await _exportTimeLogRepository.InsertAsync(new ExportTimeLog
                    {FileName = fileName, Records = timeLogs.Count});

                await _unitOfWork.SaveAsync();
            }
            catch (Exception exception)
            {
                Logger.Fatal($"Can't generate csv file CommitInsertHours", exception);
            }
        }

        #endregion

        private async Task<List<ChangedObjectDto>> CompareFiles(string oldFilePath, string newFilePath,
            ImportFileType type)
        {
            var builder = new InlineDiffBuilder(new Differ());
            var compareResult = builder
                .BuildDiffModel(await GetFileAsString(oldFilePath), await GetFileAsString(newFilePath));

            return type switch
            {
                ImportFileType.CommitFiles => GetCommitFilesObjects(compareResult),
                ImportFileType.CommitHoursBilling => GetObjects(compareResult)
            };
        }

        private List<ChangedObjectDto> GetObjects(DiffPaneModel result)
        {
            //var regex = new Regex("(?<id>\\d+)");

            var insertedLinesIds = new List<int>();
            var deletedLinesIds = new List<int>();

            var objects = new List<ChangedObjectDto>();

            foreach (var line in result.Lines.Where(l => l.Type != ChangeType.Unchanged))
            {
                try
                {
                    //var str = Convert.ToInt32(regex.Match(line.Text).Groups["id"].Value);
                    var str = GetId(line.Text);
                    var id = Convert.ToInt32(str);

                    if (line.Type == ChangeType.Inserted)
                        insertedLinesIds.Add(id);

                    if (line.Type == ChangeType.Deleted)
                        deletedLinesIds.Add(id);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }

            foreach (var item in insertedLinesIds.Intersect(deletedLinesIds))
                objects.Add(new ChangedObjectDto {Id = item, Action = RecordAction.Update});

            foreach (var item in insertedLinesIds.Except(deletedLinesIds))
                objects.Add(new ChangedObjectDto {Id = item, Action = RecordAction.Insert});

            return objects;
        }

        private List<ChangedObjectDto> GetCommitFilesObjects(DiffPaneModel result)
        {
            var regex = new Regex(
                "^\"(?<portfolioNumber>\\d{1,6}(/\\d{1,5})(.\\d{1,4}))|^\"(?<portfolioNumber>\\d{1,6}(/\\d{1,5}))|^\"(?<portfolioNumber>\\d{1,6})");

            var insertedLinesIds = new List<string>();
            var deletedLinesIds = new List<string>();

            var objects = new List<ChangedObjectDto>();

            foreach (var line in result.Lines.Where(l => l.Type != ChangeType.Unchanged))
            {
                try
                {
                    var portfolioNumber = regex.Match(line.Text).Groups["portfolioNumber"].Value;

                    if (line.Type == ChangeType.Inserted)
                        insertedLinesIds.Add(portfolioNumber);

                    if (line.Type == ChangeType.Deleted)
                        deletedLinesIds.Add(portfolioNumber);
                }
                catch
                {
                    // ignored
                }
            }

            foreach (var item in insertedLinesIds.Intersect(deletedLinesIds))
                objects.Add(new ChangedObjectDto {PortfolioNumber = item, Action = RecordAction.Update});

            foreach (var item in insertedLinesIds.Except(deletedLinesIds))
                objects.Add(new ChangedObjectDto {PortfolioNumber = item, Action = RecordAction.Insert});

            return objects;
        }

        private async Task<string> GetFileAsString(string path)
        {
            if (path is null)
                return string.Empty;

            return await Task.Run(() =>
            {
                var result = new StringBuilder();

                try
                {
                    using var streamReader = new StreamReader(path);
                    while (!streamReader.EndOfStream)
                        result.AppendLine(streamReader.ReadLine());

                    return result.ToString();
                }
                catch (Exception exception)
                {
                    Logger.Fatal($"Can't read csv file from folder!", exception);
                    return string.Empty;
                }
            });
        }

        private string GetId(string str)
        {
            var result = new StringBuilder();
            var digitFound = false;

            foreach (var res in str)
            {
                if (digitFound && !char.IsDigit(res))
                    break;

                if (char.IsDigit(res))
                {
                    result.Append(res);
                    digitFound = true;
                }
            }

            return result.ToString();
        }
    }
}