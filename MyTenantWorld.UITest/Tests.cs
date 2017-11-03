using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.Android;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace MyTenantWorld.UITest
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public const string CasablancaUsername = "admin";
        public const string CasablancaEmail = "support@gmail.com.sg";
        public const string ValidUsername = "mytenantworld@gmail.com";
        public const string ValidPassword = "P@ssword1!";
        public const string InvalidUsername = "axeljansson@gmail.com";
        public const string InvalidPassword = "aaaaaa";
        public const string Invalidemailformat = "asdasdasdasdaddas";

        static readonly Func<AppQuery, AppQuery> EmailAddress = c => c.Class("md5b60ffeb829f638581ab2bb9b1a7f4f3f.EntryEditText").Index(0);
        static readonly Func<AppQuery, AppQuery> Password = c => c.Class("md5b60ffeb829f638581ab2bb9b1a7f4f3f.EntryEditText").Index(1);
        static readonly Func<AppQuery, AppQuery> HomePage = c => c.Marked("Log out");
        static readonly Func<AppQuery, AppQuery> ActivateAccountPage = c => c.Marked("Activation");

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            //app = AppInitializer.StartApp(platform);
            const string apkpath = @"D:/Projects/MtwStaffApp/Droid/bin/Release/com.ibasetechnology.my_tenant_world.apk";
            app = ConfigureApp
                .Android
                // TODO: Update this path to point to your Android app and uncomment the
                // code if the app is not included in the solution.
                .ApkFile(apkpath)
                .StartApp();
        }

        [Test]
        public void PasswordResetTest()
        {
            // Arrange 
            app.WaitForElement(c => c.Marked("SIGN IN"), "Timeout!", TimeSpan.FromSeconds(1));
            // Act
            app.Tap(c => c.Marked("RESET MY PASSWORD"));
            // Assert
            AppResult[] result = app.WaitForElement(c => c.Text("Enter your email address below and we will send you an email with a new password."), "Reset Password Screen did not appear", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result.Any(), "Fail to navigate to Reset Password Screen.");

            // Act
            app.Tap(c => c.Marked("GO BACK"));
            // Assert
            result = app.WaitForElement(c => c.Marked("SIGN IN"), "Login Screen did not appear", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result.Any(), "Fail to navigate to Login Screen.");

            //Act
            app.Tap(c => c.Marked("RESET MY PASSWORD"));
            app.Tap(c => c.Marked("RESET PASSWORD"));
            // Assert
            result = app.WaitForElement(c => c.Id("message").Text("Please enter email."), "Blank email validation message did not appear", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result.Any(), "Blank email validation.");
            app.Tap(c => c.Marked("OK"));

            //Act - Enter invalid email address (eg. missing '@' character).
            app.EnterText(EmailAddress, Invalidemailformat);
            app.Tap(c => c.Marked("RESET PASSWORD"));
            // Assert
            result = app.WaitForElement(c => c.Id("message").Text("Email should be in valid format."), "Email format validation message did not appear", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result.Any(), "Email format validation.");
            app.Tap(c => c.Marked("OK"));

            //Act - Enter valid but non-registered email address.
            app.ClearText(EmailAddress);
            app.EnterText(EmailAddress, InvalidUsername);
            app.Tap(c => c.Marked("RESET PASSWORD"));
            // Assert
            result = app.WaitForElement(c => c.Id("message").Text("User not found"), "Non-registered email address message did not appear (within 15 seconds).", TimeSpan.FromSeconds(15));
            Assert.IsTrue(result.Any(), "Non-registered email address.");
            app.Tap(c => c.Marked("OK"));

            //Act - Enter registered email address.
            app.ClearText(EmailAddress);
            app.EnterText(EmailAddress, ValidUsername);
            app.Tap(c => c.Marked("RESET PASSWORD"));
            // Assert
            result = app.WaitForElement(c => c.Marked("Reset Successful"), "Reset Successful message did not appear (within 15 seconds).", TimeSpan.FromSeconds(15));
            Assert.IsTrue(result.Any(), "Password reset successfully.");
            app.Tap(c => c.Marked("GO BACK TO LOGIN"));
        }

        [Test]
        public void NormalLoginTest_PostAuthentication()
        {
            // Arrange
            app.WaitForElement(c => c.Marked("SIGN IN"), "Timeout!", TimeSpan.FromSeconds(1));
            // Act
            app.EnterText(EmailAddress, ValidUsername);
            app.EnterText(Password, ValidPassword);
            app.Tap(c => c.Marked("SIGN IN"));
            // Assert
            AppResult[] result = app.WaitForElement(HomePage, "Home Page Screen did not appear (within 15 seconds).", TimeSpan.FromSeconds(15));
            Assert.IsTrue(result.Any(), "The error message is not being displayed.");
            app.Screenshot("Normal Login.");

            result = app.WaitForElement(c => c.Marked("Setup"), "Setup navigation did not appear (within 1 seconds).", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result.Any(), "The error message is not being displayed.");
            result = app.WaitForElement(c => c.Marked("Audit"), "Audit navigation did not appear (within 1 seconds).", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result.Any(), "The error message is not being displayed.");
            result = app.WaitForElement(c => c.Marked("DemoPortfolio-46028"), "The DemoPortfolio did not appear (within 1 second).", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result.Any(), "The error message is not being displayed.");
            app.Tap(c => c.Marked("DemoPortfolio-46028"));
            Thread.Sleep(3000);
            app.Tap("The Palette");
            app.Tap(c => c.Marked("Log out"));
            result = app.WaitForElement(c => c.Marked("Do you want to log out?"), "Logout message did not appear (within 1 second).", TimeSpan.FromSeconds(1));
            Assert.IsTrue(result.Any(), "The logout message is not being displayed.");
            app.Tap(c => c.Marked("Yes"));
        }

        [Test]
        public async Task FirstLoginTest()
        {
            // Arrange 
            app.WaitForElement(c => c.Marked("SIGN IN"), "Timeout!", TimeSpan.FromSeconds(1));
            //Act - Call the api to reset the account to first time login account with password P@ssword1!.
            HttpClient client = new HttpClient { BaseAddress = new Uri(Config.BaseURL) };
            var json = JsonConvert.SerializeObject("mytenantworld@gmail.com");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("/api/test/firstloginreset", content);
            if (response.IsSuccessStatusCode)
            {
                Thread.Sleep(20000);
                app.EnterText(EmailAddress, ValidUsername);
                app.EnterText(Password, ValidPassword);
                app.Tap(c => c.Marked("SIGN IN"));
                // Assert
                AppResult[] result = app.WaitForElement(ActivateAccountPage, "Activate Account Page Screen did not appear (within 15 seconds).", TimeSpan.FromSeconds(15));
                Assert.IsTrue(result.Any(), "The error message is not being displayed.");
                app.Screenshot("First Time Login.");

                // Act - Hit the back button, re-login with first-time login account.
                app.Tap(c => c.Marked("GO BACK"));
                // Assert
                result = app.WaitForElement(c => c.Marked("SIGN IN"), "Login Screen did not appear", TimeSpan.FromSeconds(1));
                Assert.IsTrue(result.Any(), "Fail to navigate to Login Screen.");

                //Act - Re-login with first-time login account, tap on the Done button without entering any password.
                app.EnterText(EmailAddress, ValidUsername);
                app.EnterText(Password, ValidPassword);
                app.Tap(c => c.Marked("SIGN IN"));
                app.Tap(c => c.Marked("ACTIVATE ACCOUNT"));
                // Assert
                result = app.WaitForElement(c => c.Id("message").Text("Passwords should not be empty."), "Blank password validation message did not appear", TimeSpan.FromSeconds(1));
                Assert.IsTrue(result.Any(), "Blank passwords validation.");
                app.Tap(c => c.Marked("OK"));

                //Act - Enter a New password and a different Confirm password, then tap on Done.
                app.EnterText(c => c.Marked("Password"), "123456");
                app.EnterText(c => c.Marked("Confirm Password"), "abcdefg");
                app.Tap(c => c.Marked("ACTIVATE ACCOUNT"));
                // Assert
                result = app.WaitForElement(c => c.Id("message").Text("Passwords do not match."), "Compare both passwords validation message did not appear", TimeSpan.FromSeconds(1));
                Assert.IsTrue(result.Any(), "Compare both passwords validation.");
                app.Tap(c => c.Marked("OK"));

                //Act - Enter a New password and the same Confirm password, then tap on Done.
                app.ClearText(c => c.Marked("Password"));
                app.ClearText(c => c.Marked("Confirm Password"));
                app.EnterText(c => c.Marked("Password"), "P@ssword1!");
                app.EnterText(c => c.Marked("Confirm Password"), "P@ssword1!");
                app.Tap(c => c.Marked("ACTIVATE ACCOUNT"));
                // Assert
                result = app.WaitForElement(HomePage, "Home Page Screen did not appear (within 15 seconds).", TimeSpan.FromSeconds(15));
                Assert.IsTrue(result.Any(), "Activated account successfully.");
                app.Screenshot("First Time Login.");
            }
        }

        [Test]
        public void CondoSetupTest()
        {
            app.EnterText(EmailAddress, "support@gmail.com.sg");
            app.EnterText(Password, "P@ssword1!");
            app.Tap(c => c.Marked("SIGN IN"));
            AppResult[] result = app.WaitForElement(c => c.Marked("MenuBtn"), "Home Screen did not appear", TimeSpan.FromSeconds(50));
            Assert.IsTrue(result.Any(), "Fail to navigate to Home Screen.");
            app.Tap(c => c.Marked("MenuBtn"));
            app.Tap(c => c.Marked("Setup"));
            Thread.Sleep(3000);
            //CASABLANCA CONDOMINIUM
            var entryFields = app.Query(c => c.TextField());
            Assert.AreEqual("CASABLANCA CONDOMINIUM", entryFields[0].Text);
            Assert.AreEqual("6462 3138", entryFields[1].Text);
            Assert.AreEqual("6462 3138", entryFields[2].Text);
            var TextFields = app.Query(c => c.Class("md5b60ffeb829f638581ab2bb9b1a7f4f3f.FormsTextView"));
            Assert.AreEqual("http://www.mytenantworld.com/casablanca", TextFields[0].Text);
            app.Tap(c => c.Marked("Back"));
            Assert.AreEqual("Confirmation", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.AreEqual("Do you want to close the window?", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("Yes"));
            Thread.Sleep(3000);
            Assert.IsNotNull(app.Query(c => c.Marked("MenuBtn")));
            app.Tap(c => c.Marked("MenuBtn"));
            app.Tap(c => c.Marked("Setup"));
            Thread.Sleep(3000);
            app.ClearText(c => c.Marked("GeneralEnquiryNoEntry"));
            app.ClearText(c => c.Marked("MaintenanceNoEntry"));
            app.Tap(c => c.Marked("Done"));
            Assert.AreEqual("All Fields Required", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.AreEqual("Please fill in all fields", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("OK"));
            app.Tap(c => c.Marked("LogoChangeBtn"));
            Assert.AreEqual("Take Photos", app.Query(c => c.Marked("alertTitle"))[0].Text);
            app.Tap(c => c.Marked("Gallery"));
            Thread.Sleep(10000);
            //Select 1 photo to upload
            app.Tap(c => c.Marked("ImageChangeBtn"));
            Assert.AreEqual("Take Photos", app.Query(c => c.Marked("alertTitle"))[0].Text);
            app.Tap(c => c.Marked("Camera"));
            Thread.Sleep(10000);
            //Snap 1 photo to upload
            app.EnterText(c => c.Marked("GeneralEnquiryNoEntry"), "test");
            app.EnterText(c => c.Marked("MaintenanceNoEntry"), "test1234");
            app.Tap(c => c.Marked("Done"));
            Assert.AreEqual("Error", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.AreEqual("Contact numbers cannot contain invalid characters.", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("OK"));
            app.ClearText(c => c.Marked("GeneralEnquiryNoEntry"));
            app.ClearText(c => c.Marked("MaintenanceNoEntry"));
            app.EnterText(c => c.Marked("GeneralEnquiryNoEntry"), "64623137");
            app.EnterText(c => c.Marked("MaintenanceNoEntry"), "64623138");
            app.Tap(c => c.Marked("Done"));
            app.Tap(c => c.Marked("OK"));

            //Facilities tab test
            app.Tap(c => c.Marked("Facilities"));
            app.ScrollDown();
            Thread.Sleep(5000);
            //Delete BBQ Pit 2
            app.TapCoordinates(1490, 501);
            Assert.AreEqual("Confirmation", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.AreEqual("Are you sure to remove?", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("OK"));
            Thread.Sleep(3000);
            Assert.IsEmpty(app.Query(c => c.Marked("BBQ Pit 2")));
            app.Tap(c => c.Marked("BBQ Pit 1"));
            Thread.Sleep(5000);
            app.Tap(c => c.Marked("Slots"));
            Thread.Sleep(5000);
            app.Tap(c => c.Marked("Rules"));
            Thread.Sleep(5000);
            app.Tap(c => c.Marked("Back"));
            Assert.AreEqual("Confirmation", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.AreEqual("Do you want to close the window?", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("Yes"));
            Assert.IsNotNull(app.Query(c => c.Marked("BBQ Pit 1")));
            Assert.IsNotNull(app.Query(c => c.Marked("BBQ Pit 3")));
            Assert.IsNotNull(app.Query(c => c.Marked("BBQ Pit 4")));
            Assert.IsNotNull(app.Query(c => c.Marked("BBQ Pit 5")));
            Assert.IsNotNull(app.Query(c => c.Marked("Function Room / Reading Room")));
            Assert.IsNotNull(app.Query(c => c.Marked("Karaoke ")));
            app.Tap(c => c.Marked("Add"));
            Assert.IsNotNull(app.Query(c => c.Marked("Info")));
            Assert.IsNotNull(app.Query(c => c.Marked("Slots")));
            Assert.IsNotNull(app.Query(c => c.Marked("Rules")));

            //Facility info screen test
            //app.Tap(c => c.Marked("Done"));
            //Assert.AreEqual("All Fields Required", app.Query(c => c.Marked("alertTitle"))[0].Text);
            //Assert.AreEqual("Please fill in all fields", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("Change"));
            Assert.AreEqual("Take Photos", app.Query(c => c.Marked("alertTitle"))[0].Text);
            app.Tap(c => c.Marked("Gallery"));
            Thread.Sleep(10000);
            //Select a photo to upload
            //app.Tap(c => c.Marked("Camera"));
            //Thread.Sleep(10000);
            ////Snap a photo and upload
            app.Tap(c => c.Marked("InOperationStartDate"));
            Assert.IsNotNull(app.Query(c => c.Marked("datePicker")));
            app.Tap(c => c.Marked("Cancel"));
            app.Tap(c => c.Marked("InOperationEndDate"));
            Assert.IsNotNull(app.Query(c => c.Marked("datePicker")));
            app.Query(x => x.Marked("datePicker").Invoke("updateDate", DateTime.Now.Year, DateTime.Today.AddMonths(-1).Month, DateTime.Today.AddDays(-1).Day));
            app.Tap(c => c.Marked("OK"));
            //app.Tap(c => c.Marked("Done"));
            //Assert.AreEqual("Error", app.Query(c => c.Marked("alertTitle"))[0].Text);
            //Assert.AreEqual("The start date cannot be later than the end date", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("InOperationEndDate"));
            app.Query(x => x.Marked("datePicker").Invoke("updateDate", DateTime.Today.AddYears(5).Year, DateTime.Today.AddMonths(-1).Month, DateTime.Today.Day));
            app.Tap(c => c.Marked("OK"));
            app.Tap(c => c.Marked("B"));
            //Assert.AreEqual("BBQ Pit 1, BBQ Pit 3, BBQ Pit 4, BBQ Pit 5", app.Query(c => c.Marked("alertTitle"))[0].Text);
            app.Tap(c => c.Marked("C"));
            //Assert.AreEqual("Tennis Court  1, Tennis Court 2", app.Query(c => c.Marked("alertTitle"))[0].Text);
            app.Tap(c => c.Marked("+"));
            Assert.IsNotNull(app.Query(c => c.Marked("Capacity: 1")));
            app.Tap(c => c.Marked("-"));
            Assert.IsNotNull(app.Query(c => c.Marked("Capacity: 0")));
            app.Tap(c => c.Marked("+"));
            app.Tap(c => c.Marked("+"));
            app.Tap(c => c.Marked("+"));
            app.Tap(c => c.Marked("+"));
            app.Tap(c => c.Marked("+"));
            app.EnterText(c => c.Marked("FacilityNameEntry"), "BBQ Pit 2");
            app.EnterText(c => c.Marked("DescriptionEntry"), "BBQ Pit 2 Description");
            app.EnterText(c => c.Marked("TermConditionEntry"), "BBQ Pit 2 Terms & Conditions");
            app.EnterText(c => c.Marked("EquipmentEntry"), "BBQ Pit 2 Equipment");
            app.EnterText(c => c.Marked("DepositEntry"), "100");
            app.EnterText(c => c.Marked("ContactEntry"), "Coco");
            app.EnterText(c => c.Marked("EmailEntry"), "seet-sun.leeibase.com.sg");
            app.EnterText(c => c.Marked("PhoneEntry"), "98764567");
            app.EnterText(c => c.Marked("FaxEntry"), "68973456");
            app.Tap(c => c.Marked("Done"));
            Assert.AreEqual("Invalid Email", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.AreEqual("Please enter a valid email address", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("OK"));
            app.ClearText(c => c.Marked("EmailEntry"));
            app.EnterText(c => c.Marked("EmailEntry"), "seet-sun.lee@ibase.com.sg");
            //app.Tap(c => c.Marked("Done"));
            //Assert.AreEqual("Success", app.Query(c => c.Marked("alertTitle"))[0].Text);
            //Assert.AreEqual("Facility has been saved successfully", app.Query(c => c.Marked("message"))[0].Text);
            //app.Tap(c => c.Marked("OK"));
            //app.Tap(c => c.Marked("BBQ Pit 2"));

            //Booking slot screen test
            app.Tap(c => c.Marked("Slots"));
            app.Tap(c => c.Marked("Back"));
            Assert.AreEqual("Confirmation", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.AreEqual("Do you want to close the window?", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("No"));
            app.Tap(c => c.Marked("Add"));
            ////asset new slot added
            //app.EnterText(c => c.Marked("StartTimeEntry"), "6:00 PM");
            //app.EnterText(c => c.Marked("EndTimeEntry"), "10:00 PM");
            //app.EnterText(c => c.Marked("FeesEntry"), "50");
            app.Tap(c => c.Marked("Add"));
            app.Tap(c => c.Marked("Clone"));
            ////assert popup of slot day selection
            //app.Tap(c => c.Marked("Monday"));
            //app.Tap(c => c.Marked("Sunday"));
            ////assert Monday slots have been copied to Sunday
            app.SwipeRightToLeft();//Tuesday
            app.SwipeRightToLeft();//wednesday
            app.SwipeRightToLeft();//Thursday
            app.SwipeRightToLeft();//Friday
            app.SwipeRightToLeft();//Saturday
            app.SwipeRightToLeft();//Sunday
            app.TapCoordinates(28, 254); //Delete button of first slot
            //Assert.AreEqual("Delete", app.Query(c => c.Marked("alertTitle"))[0].Text);
            //Assert.AreEqual("Are you sure you want to delete this slot?", app.Query(c => c.Marked("message"))[0].Text);
            //app.Tap(c => c.Marked("OK"));
            app.TapCoordinates(308, 255); //Standard/ Prime switch
            //Enter invalid data
            //clear invalid data
            //enter valid data
            //app.Tap(c => c.Marked("Done"));

            //Booking rules screen test
            app.Tap(c => c.Marked("Rules"));
            app.Tap(c => c.Marked("Back"));
            Assert.AreEqual("Confirmation", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.AreEqual("Do you want to close the window?", app.Query(c => c.Marked("message"))[0].Text);
            app.Tap(c => c.Marked("No"));
            app.TapCoordinates(app.Query(c => c.Marked("+"))[0].Rect.X, app.Query(c => c.Marked("+"))[0].Rect.Y);
            app.TapCoordinates(app.Query(c => c.Marked("-"))[0].Rect.X, app.Query(c => c.Marked("+"))[0].Rect.Y);
            app.TapCoordinates(app.Query(c => c.Marked("+"))[1].Rect.X, app.Query(c => c.Marked("+"))[1].Rect.Y);
            app.TapCoordinates(app.Query(c => c.Marked("-"))[1].Rect.X, app.Query(c => c.Marked("+"))[1].Rect.Y);
            app.TapCoordinates(app.Query(c => c.Marked("+"))[2].Rect.X, app.Query(c => c.Marked("+"))[2].Rect.Y);
            app.TapCoordinates(app.Query(c => c.Marked("-"))[2].Rect.X, app.Query(c => c.Marked("+"))[2].Rect.Y);
            //Incomplete step 4 t0 27
            //app.Tap(c => c.Marked("Back"));
            //Assert.AreEqual("Confirmation", app.Query(c => c.Marked("alertTitle"))[0].Text);
            //Assert.AreEqual("Do you want to close the window?", app.Query(c => c.Marked("message"))[0].Text);
            //app.Tap(c => c.Marked("Yes"));

            //Committee Setup
            app.Tap(c => c.Marked("Committee"));
            Thread.Sleep(3000);
            TextFields = app.Query(c => c.Class("md5b60ffeb829f638581ab2bb9b1a7f4f3f.FormsTextView"));
            Assert.AreEqual("KRISHNAN HARIHARA KASTHURI RANGAN / HARIHARA KASTHURI RANGAN J", TextFields[0].Text);
            Assert.AreEqual("Member", TextFields[1].Text);
            Assert.AreEqual("NEO BEE KIONG / TAN SWEE CHOO", TextFields[2].Text);
            Assert.AreEqual("Member", TextFields[3].Text);
            Assert.AreEqual("TAN KA HONG/TAY KIM THIAM", TextFields[4].Text);
            Assert.AreEqual("Member", TextFields[5].Text);
            app.Tap(c => c.Marked("Add"));
            //New committee added with default values.
            app.TapCoordinates(1051, 207);
            //Delete committee
            app.Tap(c => c.Marked("Done"));

            //Customisation Setup
            app.Tap(c => c.Marked("Customisation"));
        }

        [Test]
        public async Task UsernameNormalNFirstLoginTest()
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(Config.BaseURL) };
            var json = JsonConvert.SerializeObject("admin");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("/api/test/firstloginreset", content);
            if (response.IsSuccessStatusCode)
            {
                Thread.Sleep(20000);
                app.EnterText(EmailAddress, "admin");
                app.EnterText(Password, ValidPassword);
                app.Tap(c => c.Marked("SIGN IN"));
                // Assert
                AppResult[] result = app.WaitForElement(ActivateAccountPage, "Activate Account Page Screen did not appear (within 50 seconds).", TimeSpan.FromSeconds(50));
                Assert.IsTrue(result.Any(), "The error message is not being displayed.");

                app.EnterText(c => c.Marked("Password"), ValidPassword);
                app.EnterText(c => c.Marked("Confirm Password"), ValidPassword);
                app.Tap(c => c.Marked("ACTIVATE ACCOUNT"));
                result = app.WaitForElement(c => c.Marked("MenuBtn_Container"), "Home Page Screen did not appear (within 50 seconds).", TimeSpan.FromSeconds(50));
                Assert.IsTrue(result.Any(), "Activated account successfully.");
                app.Tap(c => c.Marked("MenuBtn"));
                app.Tap(c => c.Marked("Log out"));
                app.Tap(c => c.Marked("Yes"));
                Assert.IsNotNull(app.Query(c => c.Marked("RESET MY PASSWORD")));
                app.EnterText(EmailAddress, "admin");
                app.EnterText(Password, ValidPassword);
                app.Tap(c => c.Marked("SIGN IN"));

                result = app.WaitForElement(c => c.Marked("MenuBtn_Container"), "Home Page Screen did not appear (within 50 seconds).", TimeSpan.FromSeconds(50));
                Assert.IsTrue(result.Any(), "Activated account successfully.");
                app.Screenshot("First Time Login for admin username.");
            }
        }

        [Test]
        public void UserSetupTest()
        {
        }

        [Test]
        public void FileRepositoryTest()
        {
        }

        [Test]
        public void FacilityBooking()
        {
            //Booking for resident
            app.EnterText(EmailAddress, CasablancaUsername);
            app.EnterText(Password, ValidPassword);
            app.Tap(c => c.Marked("SIGN IN"));
            AppResult[] result = app.WaitForElement(c => c.Marked("MenuBtn_Container"), "Home Page Screen did not appear (within 50 seconds).", TimeSpan.FromSeconds(50));
            Assert.IsTrue(result.Any(), "Login successfully.");
            app.Tap(c => c.Marked("Book Resident"));
            Thread.Sleep(5000);
            //Assert.IsNotNull(app.Query(c => c.Marked("CASABLANCA CONDOMINIUM Facilities")));
            app.Back();
            result = app.WaitForElement(c => c.Marked("MenuBtn_Container"), "Home Page Screen did not appear (within 50 seconds).", TimeSpan.FromSeconds(50));
            Assert.IsTrue(result.Any(), "Login successfully.");
            app.Tap(c => c.Marked("Book Resident"));
            Thread.Sleep(5000);
            app.ScrollDown();
            app.Tap(c => c.Marked("Select Block"));
            Assert.AreEqual("Select Block", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.IsNotNull(app.Query(c => c.Marked("21A")));
            Assert.IsNotNull(app.Query(c => c.Marked("23B")));
            Assert.IsNotNull(app.Query(c => c.Marked("25C")));
            Assert.IsNotNull(app.Query(c => c.Marked("27D")));
            Assert.IsNotNull(app.Query(c => c.Marked("29E")));
            Assert.IsNotNull(app.Query(c => c.Marked("31F")));
            app.Tap(c => c.Marked("21A"));
            Thread.Sleep(10000);
            Assert.IsNotNull(app.Query(c => c.Marked("21A")));
            app.Tap(c => c.Marked("Select Unit"));
            Assert.AreEqual("Select Unit", app.Query(c => c.Marked("alertTitle"))[0].Text);
            Assert.IsNotNull(app.Query(c => c.Marked("#02-06")));
            Assert.IsNotNull(app.Query(c => c.Marked("#04-02")));
            Assert.IsNotNull(app.Query(c => c.Marked("#01-05")));
            Assert.IsNotNull(app.Query(c => c.Marked("#05-03")));
            Assert.IsNotNull(app.Query(c => c.Marked("#03-04")));
            app.Tap(c => c.Marked("#01-06"));
            Thread.Sleep(10000);
            Assert.IsNotNull(app.Query(c => c.Marked("#01-06")));
            //Assert.IsNotNull(app.Query(c => c.Marked("No tenant found")));
            //app.Tap(c => c.Marked("No tenant found"));
            app.Tap(c => c.Marked("#01-06"));
            app.Tap(c => c.Marked("#04-03"));
            Thread.Sleep(10000);
            Assert.IsNotNull(app.Query(c => c.Marked("#04-03")));
            app.Tap(c => c.Marked("Select Tenant"));
            app.Tap(c => c.Marked("MARAPPA SIVABALAN VISHNU & GOWRI MANOKARI (TENANT) TA FROM 16 Mar 2017 to 15 Feb 2018 "));
            Assert.IsNotNull(app.Query(c => c.Marked("MARAPPA SIVABALAN VISHNU & GOWRI MANOKARI (TENANT) TA FROM 16 Mar 2017 to 15 Feb 2018 ")));
            app.Tap(c => c.Marked("Tennis Court 1"));
        }

        [Test]
        public void Feedback()
        {
        }

        [Test]
        public void Visitors()
        {
        }

        [Test]
        public void News()
        {
        }

        [Test]
        public void ReceiptsRefunds()
        {
        }

        [Test]
        public void Audit()
        {
        }
    }
}

