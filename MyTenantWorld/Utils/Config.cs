﻿namespace MyTenantWorld
{
    public static class Config
    {
#if DEBUG
        public static string BaseURL = "https://mtw-uat.azurewebsites.net";
        public static string LoginURL = "https://mtwauthentication-uat.azurewebsites.net/token";
#else
        public static string BaseURL = "https://mtw.azurewebsites.net";
        //public static string BaseURL = "https://mtw-development.azurewebsites.net"; // For coco
        public static string LoginURL = "https://mtwauthentication.azurewebsites.net/token";
        //public static string LoginURL = "https://mtwauthentication-development.azurewebsites.net/token"; // For coco
#endif
        public static string CommonErrorMsg = "An unexpected problem has occured, please try again later";
        public static string EmptyValidationMsg = "Please fill in all fields";
        public static string InvalidEmailFormatMsg = "Please enter a valid email address";
        public static string InvalidEmailFormatTitle = "Invalid Email";
        public static string AllFieldsReqTitle = "All Fields Required";
        public static string OopsTitle = "Oops!";
        public static string SuccessfulSaveMsg = "Data has been saved successfully!";
        public static string PortfolioEditorURL = "http://mtw.azurewebsites.net/";
        public static string NotStrongPassword = "Your password isn't strong enough";
        public static string DateValidatinMsg = "The start date cannot be later than the end date";
        public static string ClientID = "af567d81f67141bda52960ee9cc03deb.mtwstaffapp.com.ibase";
        public static string ConfirmDiscardMsg = "Are you going to discard the changes?";
        public static string ConfirmDeleteMsg = "Are you sure to remove?";
    }
}