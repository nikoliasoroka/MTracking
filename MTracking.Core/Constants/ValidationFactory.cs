namespace MTracking.Core.Constants
{
    public static class ValidationFactory
    {
        public const string AccessDenied = "ACCESS_DENIED";
        public const string WrongPassword = "WRONG_PASSWORD";
        public const string WrongVerificationCode = "WRONG_VERIFICATION_CODE";
        public const string TheAccessIsRestricted = "THE_ACCESS_IS_RESTRICTED";
        public const string PasswordIsNotChanged = "PASSWORD_IS_NOT_CHANGED";
        public const string NotEnoughPermissionToTrackTheTime = "NOT_ENOUGH_PERMISSION_TO_TRACK_THE_TIME";

        public const string ClaimsIsNotCreated = "CLAIMS_IS_NOT_CREATED";
        public const string ExpiredTokenPrincipalIsNotGotten = "EXPIRED_TOKEN_PRINCIPAL_IS_NOT_GOTTEN";
        public const string AccessTokenIsNotCreated = "ACCESS_TOKEN_IS_NOT_CREATED";
        public const string RefreshTokenIsNotCreated = "REFRESH_TOKEN_IS_NOT_CREATED";
        public const string TokenIsNotGenerated = "TOKEN_IS_NOT_GENERATED";
        public const string InvalidToken = "INVALID_TOKEN";

        public const string EmailMessageIsNotCreated = "EMAIL_MESSAGE_IS_NOT_CREATED";
        public const string EmailIsNotSent = "EMAIL_IS_NOT_SENT";
        public const string EmailDoesNotExist = "EMAIL_DOES_NOT_EXIST";

        public const string UserAlreadyExists = "USER_ALREADY_EXISTS";
        public const string UserIsNotCreated = "USER_IS_NOT_CREATED";
        public const string UserIsNotFound = "USER_IS_NOT_FOUND";

        public const string FileIsNotCreated = "FILE_IS_NOT_CREATED";
        public const string FileIsNotUpdated = "FILE_IS_NOT_UPDATED";
        public const string FileIsNotFound = "FILE_IS_NOT_FOUND";
        public const string FileIsClosed = "FILE_IS_CLOSED";
        public const string FileIsNotDeleted = "FILE_IS_NOT_DELETED";
        public const string NonBillableCase = "NON_BILLABLE_CASE";

        public const string TopicIsNotCreated = "TOPIC_IS_NOT_CREATED";
        public const string TopicIsNotUpdated = "TOPIC_IS_NOT_UPDATED";
        public const string TopicIsNotFound = "TOPIC_IS_NOT_FOUND";
        public const string TopicIsNotDeleted = "TOPIC_IS_NOT_DELETED";

        public const string DescriptionIsNotCreated = "DESCRIPTION_IS_NOT_CREATED";
        public const string DescriptionIsNotUpdated = "DESCRIPTION_IS_NOT_UPDATED";
        public const string DescriptionIsNotFound = "DESCRIPTION_IS_NOT_FOUND";
        public const string DescriptionIsNotDeleted = "DESCRIPTION_IS_NOT_DELETED";

        public const string TimeLogIsNotCreated = "TIMELOG_IS_NOT_CREATED";
        public const string TimeLogIsNotUpdated = "TIMELOG_IS_NOT_UPDATED";
        public const string TimeLogIsNotFound = "TIMELOG_IS_NOT_FOUND";
        public const string TimeLogIsNotDeleted = "TIMELOG_IS_NOT_DELETED";
        public const string NotAllowedAmountOfTime = "NOT_ALLOWED_AMOUNT_OF_TIME";
        public const string NotAllowedDate = "NOT_ALLOWED_DATE";
        public const string RecordIsCharged = "RECORD_IS_CHARGED";

        public const string ReminderIsNotCreated = "REMINDER_IS_NOT_CREATED";
        public const string ReminderIsNotUpdated = "REMINDER_IS_NOT_UPDATED";
        public const string ReminderIsNotFound = "REMINDER_IS_NOT_FOUND";
        public const string ReminderIsNotDeleted = "REMINDER_IS_NOT_DELETED";
        public const string CannotCreateMoreThan5Reminders = "CANNOT_CREATE_MORE_THAN_5_REMINDERS";

        public const string TimerIsNotCreated = "TIMER_IS_NOT_CREATED";
        public const string TimerIsNotFound = "TIMER_IS_NOT_FOUND";
        public const string TimerAlreadyExists = "TIMER_ALREADY_EXISTS";
        public const string TimerIsNotStopped = "TIMER_IS_NOT_STOPPED";
        public const string TimerIsStopped = "TIMER_IS_STOPPED";
        public const string TimerIsNotStarted = "TIMER_IS_NOT_STARTED";
        public const string TimerIsNotReset = "TIMER_IS_NOT_RESET";

        public const string DeviceIsNotRegistered = "DEVICE_IS_NOT_REGISTERED";
        public const string DeviceIsNotUpdated = "DEVICE_IS_NOT_UPDATED";

        public const string FirebaseError = "FIREBASE_ERROR";

        public const string FileWasPinned = "FILE_WAS_PINNED";
        public const string FileWasUnPinned = "FILE_WAS_UNPINNED";
        public const string ErrorWhilePinningFile = "FILE_WASNT_PINNED";
    }
}