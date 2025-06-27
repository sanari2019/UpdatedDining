using RepoDb;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DiningVsCodeNew;
using Microsoft.Extensions.Configuration;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.StatementBuilders;
using DiningVsCodeNew.Models;


namespace DiningVsCodeNew
{

    public class UserRepository : BaseRepository<User, SqlConnection>
    {

        //Setting cstring=new Setting();
        Setting sett = new Setting();
        public UserRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);

        }
        public void insertUser(User us)
        {
            //UserRepository usrrepository = new UserRepository(cstring.ConString);
            this.Insert(us);
        }
        public void updateUser(User us)
        {

            this.Update(us);
        }
        public int deleteUser(User us)
        {

            int id = this.Delete<User>(us);
            return id;
        }
        public List<User> GetUsers()
        {

            var users = new List<User>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                users = connection.QueryAll<User>().ToList();
                /* Do the stuffs for the people here */
            }
            return users;
        }
        public List<User> GetUserByUsernamePattern(string usernamePattern)
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(sett.ConString))
            {

                users = connection.ExecuteQuery<User>("[dbo].[usp_getUserByUsernamePattern]", new { UsernamePattern = usernamePattern }, // Pass the parameters
                            commandType: System.Data.CommandType.StoredProcedure
                        ).ToList();
            }
            return users;
        }
        public User GetUserByUsername(string usernameP)
        {
            // User user = null; // Initialize user variable

            // SQL query to select user by username
            string query = "SELECT * FROM [Dining].[dbo].[user] WHERE userName = @Username";

            using (var connection = new SqlConnection(sett.ConString))
            {

                return connection.ExecuteQuery<User>(query, new { userName = usernameP }
                        ).FirstOrDefault();
            }

        }

        public User GetUser(int id)
        {

            var user = new User();
            using (var connection = new SqlConnection(sett.ConString))
            {

                user = connection.Query<User>(id).FirstOrDefault();
            }
            return user;
        }
        public User GetUserName(string username)
        {

            var user = new User();
            using (var connection = new SqlConnection(sett.ConString))
            {
                user = connection.Query<User>(e => e.userName == username).FirstOrDefault();
            }
            return user;
        }
        public User GetUserByCustId(string custId)
        {

            var user = new User();
            using (var connection = new SqlConnection(sett.ConString))
            {
                user = connection.Query<User>(e => e.custId == custId).FirstOrDefault();
            }
            return user;
        }

    }
    public class ServedRepository : BaseRepository<Served, SqlConnection>
    {

        //Setting cstring=new Setting();
        Setting sett = new Setting();
        public ServedRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);

        }
        public void insertServed(Served serv)
        {
            //UserRepository usrrepository = new UserRepository(cstring.ConString);
            this.Insert(serv);
        }
        public void updateServed(Served serv)
        {

            this.Update(serv);
        }
        public int deleteServed(Served serv)
        {
            int id = 0;
            using (var connection = new SqlConnection(ConnectionString))
            {
                var deletedRows = connection.Delete<Served>(p => p.Id == serv.Id);
                id = deletedRows;
            }

            return id;
        }
        public List<Served> GetServeds()
        {

            var serveds = new List<Served>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "SELECT * FROM [dbo].[Served] Where isserved=0";
                serveds = connection.ExecuteQuery<Served>(sql).ToList();
                // serveds = connection.QueryAll<Served>(e => e.isServed == 0).ToList();
                foreach (Served sv in serveds)
                {
                    sv.paymentMain = connection.Query<PaymentMain>(sv.paymentMainId).FirstOrDefault();
                }
            }
            return serveds;
        }
        public int GetServedMealsCount()
        {

            int servedcount = 0;
            using (var connection = new SqlConnection(sett.ConString))
            {
                connection.Open();
                servedcount = connection.ExecuteQuery<int>("SELECT TotalServedMeals FROM ServedMealsCountView").FirstOrDefault();


            }
            return servedcount;
        }

         public int GetServedMealsDCount(string mealType)
    {
        int servedCount = 0;

        using (var connection = new SqlConnection(sett.ConString))
        {
            connection.Open();

            var query = "SELECT TotalServedMeals FROM ServedMealsDetailedCountView WHERE MealType = @MealType";
            servedCount = connection.ExecuteQuery<int>(query, new { MealType = mealType }).FirstOrDefault();
        }

        return servedCount;
    }

    public int GetBreakfastCount()
    {
        return GetServedMealsDCount("Breakfast");
    }

    public int GetLunchCount()
    {
        return GetServedMealsDCount("Lunch");
    }

    public int GetDinnerCount()
    {
        return GetServedMealsDCount("Dinner");
    }

        public List<HistoryRecord> GetHistoryRecords(int ServedBy)
        {

            var history = new List<HistoryRecord>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var parameters = new
                {
                    @ServedBy = ServedBy,
                };

                // Execute the stored procedure with parameters
                history = connection.ExecuteQuery<HistoryRecord>(
                    "[dbo].[usp_getHistoryRecords]",
                    new { ServedBy = ServedBy }, // Pass the parameters
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return history;
            }
        }
        public List<Served> GetServedbyCustomer(int id)
        {

            var serveds = new List<Served>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "SELECT * FROM [dbo].[Served] S inner join paymentmain P on S.paymentmainid=P.id Where isserved=0 and P.enteredby=@id";
                serveds = connection.ExecuteQuery<Served>(sql, new { id = id }).ToList();
                // serveds = connection.QueryAll<Served>(e => e.isServed == 0).ToList();
                foreach (Served sv in serveds)
                {
                    sv.paymentMain = connection.Query<PaymentMain>(sv.paymentMainId).FirstOrDefault();
                }
            }
            return serveds;
        }
        public List<Served> GetTopNServed(int enteredby)
        {

            var serveds = new List<Served>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                // var sql = "SELECT * FROM [dbo].[Served] S inner join paymentmain P on S.paymentmainid=P.id Where isserved=0 and P.enteredby=@id";
                serveds = connection.ExecuteQuery<Served>("[dbo].[usp_GetTopNServed]", new { enteredby = enteredby }).ToList();
                // serveds = connection.QueryAll<Served>(e => e.isServed == 0).ToList();
                foreach (Served sv in serveds)
                {
                    sv.paymentMain = connection.Query<PaymentMain>(sv.paymentMainId).FirstOrDefault();
                }
            }
            return serveds;
        }
        public Served GetServed(int id)
        {

            var served = new Served();
            using (var connection = new SqlConnection(sett.ConString))
            {

                served = connection.Query<Served>(id).FirstOrDefault();
            }
            return served;
        }
        // public Served GetServed(string username)
        // {  

        //    var user= new User();
        //    using (var connection = new SqlConnection(sett.ConString))
        //     {
        //        user = connection.Query<User>(e=>e.userName==username).FirstOrDefault();
        //     }
        //   return user;
        // }
         public Vw_PaymentVsServed GetPymtUnserved(int id)
        {

            var pymt = new Vw_PaymentVsServed();
            using (var connection = new SqlConnection(sett.ConString))
            {

                pymt = connection.Query<Vw_PaymentVsServed>(id).FirstOrDefault();
            }
            return pymt;
        }


        public List<ServedReportModel> GetServedReport(DateTime startDate, DateTime endDate)
        {
            List<ServedReportModel> servedreport = new List<ServedReportModel>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var parameters = new
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                servedreport = connection.ExecuteQuery<ServedReportModel>(
                    "[dbo].[usp_getServedReport]",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
            }
            return servedreport;
        }

        public List<UnservedReportModel> GetUnservedReport(DateTime startDate, DateTime endDate)
        {
            List<UnservedReportModel> unservedreport = new List<UnservedReportModel>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var parameters = new
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                unservedreport = connection.ExecuteQuery<UnservedReportModel>(
                    "[dbo].[usp_getUnservedReport]",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
            }
            return unservedreport;
        }
        public List<ServedSummaryReportModel> GetServedSummaryReport(DateTime startDate, DateTime endDate)
        {
            List<ServedSummaryReportModel> servedsummaryreport = new List<ServedSummaryReportModel>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var parameters = new
                {
                    @startDate = startDate,
                    @endDate = endDate
                };

                servedsummaryreport = connection.ExecuteQuery<ServedSummaryReportModel>(
                    "usp_getServedSummaryReport",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
            }
            return servedsummaryreport;
        }



    }
    public class OnlinePaymentRepository : BaseRepository<OnlinePayment, SqlConnection>
    {

        //Setting cstring=new Setting();
        Setting sett = new Setting();
        public OnlinePaymentRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);

        }
        public void insertOnlinePayment(OnlinePayment onlinepymt)
        {
            //UserRepository usrrepository = new UserRepository(cstring.ConString);
            using (var connection = new SqlConnection(sett.ConString))
            {
                connection.Insert(onlinepymt);
            }
        }
        public void updateOnlinePayment(OnlinePayment onlinepymt)
        {
            this.Update(onlinepymt);

        }
        public int deleteOnlinePayment(OnlinePayment onlinepymt)
        {

            int id = this.Delete<OnlinePayment>(onlinepymt);
            return id;
        }
        public List<OnlinePayment> GetOnlinePayments()
        {

            var onlinepayments = new List<OnlinePayment>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                onlinepayments = connection.QueryAll<OnlinePayment>().ToList();
                /* Do the stuffs for the people here */
            }
            return onlinepayments;
        }
        public OnlinePayment GetOnlinePayment(int id)
        {

            var onlinepayment = new OnlinePayment();
            using (var connection = new SqlConnection(sett.ConString))
            {

                onlinepayment = connection.Query<OnlinePayment>(id).FirstOrDefault();
            }
            return onlinepayment;
        }
        // public User GetOnlinePayment(string username)
        // {  

        //    var user= new User();
        //    using (var connection = new SqlConnection(sett.ConString))
        //     {
        //        user = connection.Query<User>(e=>e.userName==username).FirstOrDefault();
        //     }
        //   return user;
        // }

        public List<TotalRevenueModel> GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            List<TotalRevenueModel> totalrev = new List<TotalRevenueModel>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var parameters = new
                {
                    @startDate = startDate,
                    @endDate = endDate
                };

                totalrev = connection.ExecuteQuery<TotalRevenueModel>(
                    "usp_getTotalRevenue",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
            }
            return totalrev;
        }

        public OnlinePayment GetOnlinePaymentByRefNo(string transRefNo)
        {

            OnlinePayment onlinePayment = new OnlinePayment();

            using (var connection = new SqlConnection(sett.ConString))
            {
                var query = "SELECT * FROM [OnlinePayment] WHERE TransRefNo = @TransRefNo";
                onlinePayment = connection.ExecuteQuery<OnlinePayment>(query, new { TransRefNo = transRefNo }).FirstOrDefault();
            }

            return onlinePayment;
        }


    }


    public class CustomerTypeRepository : BaseRepository<CustomerType, SqlConnection>
    {

        Setting sett = new Setting();
        public CustomerTypeRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertCustomerType(CustomerType CustType)
        {

            this.Insert(CustType);
        }
        public void updateCustomerType(CustomerType CustType)
        {

            this.Update(CustType);
        }
        public int deleteCustomerType(CustomerType CustType)
        {
            //CustomerTypeRepository custTypeRepository = new CustomerTypeRepository(cstring);
            int id = this.Delete<CustomerType>(CustType);
            return id;
        }
        public List<CustomerType> GetCustomerTypes()
        {

            //var user=new User();
            //  Setting sett=new Setting();
            List<CustomerType> CustomerTypes = new List<CustomerType>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "select id,Name from CustomerType where isvisible=1";
                CustomerTypes = connection.ExecuteQuery<CustomerType>(sql).ToList();
                /* Do the stuffs for the people here */
            }
            return CustomerTypes;
        }

    }
    public class CustomerRouteRepository : BaseRepository<CustomerRoute, SqlConnection>
    {

        Setting sett = new Setting();
        public CustomerRouteRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertCustomerRoute(CustomerRoute CustRoute)
        {

            this.Insert(CustRoute);
        }
        public void updateCustomerType(CustomerRoute CustRoute)
        {

            this.Update(CustRoute);
        }
        public int deleteCustomerType(CustomerRoute CustRoute)
        {
            //CustomerTypeRepository custTypeRepository = new CustomerTypeRepository(cstring);
            int id = this.Delete<CustomerRoute>(CustRoute);
            return id;
        }
        public List<Route> GetCustomerRoutes(User us)
        {

            //var user=new User();
            //  Setting sett=new Setting();
            List<Route> routes = new List<Route>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "select id,path,menuname FROM [dbo].[Route] Where id in (select routeid from customerroute where custtypeid=@custtypeid)";
                routes = connection.ExecuteQuery<Route>(sql, new { custtypeid = us.custTypeId }).ToList();
                /* Do the stuffs for the people here */
            }
            return routes;
        }

    }
    public class PaymentModeRepository : BaseRepository<PaymentMode, SqlConnection>
    {

        Setting sett = new Setting();
        public PaymentModeRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertPaymentMode(PaymentMode pymtMode)
        {

            this.Insert(pymtMode);
        }
        public void updatePaymentMode(PaymentMode pymtMode)
        {

            this.Update(pymtMode);
        }
        public int deletePaymentMode(PaymentMode pymtMode)
        {
            //CustomerTypeRepository custTypeRepository = new CustomerTypeRepository(cstring);
            int id = this.Delete<PaymentMode>(pymtMode);
            return id;
        }
        public List<PaymentMode> GetPaymentModes()
        {

            //var user=new User();
            //  Setting sett=new Setting();
            List<PaymentMode> paymentmodes = new List<PaymentMode>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                paymentmodes = connection.QueryAll<PaymentMode>().ToList();
                /* Do the stuffs for the people here */
            }
            return paymentmodes;
        }

    }
    public class OrderedMealRepository : BaseRepository<OrderedMeal, SqlConnection>
    {

        Setting sett = new Setting();
        public OrderedMealRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertOrderedMeal(OrderedMeal orderedMeal)
        {

            this.Insert(orderedMeal);
        }
        public void updateOrderedMeal(OrderedMeal orderedMeal)
        {

            this.Update(orderedMeal);
        }
        public int deleteOrderedMeal(OrderedMeal orderedMeal)
        {
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "delete FROM [dbo].[orderedmeal]  Where  id=@Id";
                int id = connection.ExecuteNonQuery(sql, new { id = orderedMeal.Id });
                return id;
            }

        }
        public List<OrderedMeal> GetOrderedMeals()
        {

            //var user=new User();
            //  Setting sett=new Setting();
            List<OrderedMeal> orderedMeals = new List<OrderedMeal>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                orderedMeals = connection.QueryAll<OrderedMeal>().ToList();
                /* Do the stuffs for the people here */
            }
            return orderedMeals;
        }
        public List<OrderedMeal> GetOrderedMealsbyCust(User us)
        {

            List<OrderedMeal> orderedMeals = new List<OrderedMeal>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "SELECT * FROM [dbo].[orderedmeal]  Where submitted=0 and enteredby=@enteredby";
                orderedMeals = connection.ExecuteQuery<OrderedMeal>(sql, new { Enteredby = us.id }).ToList();
                // serveds = connection.QueryAll<Served>(e => e.isServed == 0).ToList();
                foreach (OrderedMeal ord in orderedMeals)
                {
                    ord.menu = connection.Query<Menu>(ord.MealId).FirstOrDefault();
                }
            }
            return orderedMeals;
        }
        public List<OrderedMeal> GetPaidOrderedMeals(PaymentMain pymain)
        {

            List<OrderedMeal> orderedMeals = new List<OrderedMeal>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "SELECT * FROM [dbo].[orderedmeal]  Where submitted=1 and paymentMainId=@paymentMainId";
                orderedMeals = connection.ExecuteQuery<OrderedMeal>(sql, new { paymentMainId = pymain.Id }).ToList();
                // serveds = connection.QueryAll<Served>(e => e.isServed == 0).ToList();
                foreach (OrderedMeal ord in orderedMeals)
                {
                    ord.menu = connection.Query<Menu>(ord.MealId).FirstOrDefault();
                }
            }
            return orderedMeals;
        }
        public List<ServedAlacarteVoucherModel> ExecuteAlacarteOrders(DateTime startdate, DateTime enddate)
        {
            List<ServedAlacarteVoucherModel> orderedMeals = new List<ServedAlacarteVoucherModel>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                // Define the parameters for the stored procedure
                var parameters = new
                {
                    @startdate = startdate,
                    @enddate = enddate
                };

                orderedMeals = connection.ExecuteQuery<ServedAlacarteVoucherModel>(
                    "[dbo].[usp_getServedAlacarteVoucher]",
                    parameters, // Pass the parameters
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();
            }
            return orderedMeals;
        }




    }

    public class AvailableMealRepository : BaseRepository<AvailableMeal, SqlConnection>
    {
        Setting sett = new Setting();

        public AvailableMealRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }

        public void InsertAvailableMeal(AvailableMeal availableMeal)
        {
            this.Insert(availableMeal);
        }

        public void UpdateAvailableMeal(AvailableMeal availableMeal)
        {
            this.Update(availableMeal);
        }

        public int DeleteAvailableMeal(AvailableMeal availableMeal)
        {
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "DELETE FROM [dbo].[AvailableMeal] WHERE Id = @Id";
                int id = connection.ExecuteNonQuery(sql, new { Id = availableMeal.Id });
                return id;
            }
        }

        public List<AvailableMeal> GetAllAvailableMeals()
        {
            List<AvailableMeal> availableMeals = new List<AvailableMeal>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                availableMeals = connection.QueryAll<AvailableMeal>().ToList();
            }
            return availableMeals;
        }

        public List<AvailableMeal> GetActiveMeals()
        {
            List<AvailableMeal> activeMeals = new List<AvailableMeal>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "SELECT * FROM [dbo].[AvailableMeal] WHERE Active = 1 ORDER BY Id asc";
                activeMeals = connection.ExecuteQuery<AvailableMeal>(sql).ToList();
            }
            return activeMeals;
        }
        public List<AvailableMeal> GetInactiveMeals()
        {
            List<AvailableMeal> activeMeals = new List<AvailableMeal>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "SELECT * FROM [dbo].[AvailableMeal] WHERE Active = 0 ORDER BY Id asc";
                activeMeals = connection.ExecuteQuery<AvailableMeal>(sql).ToList();
            }
            return activeMeals;
        }
    }

    public class MealActivityRepository : BaseRepository<MealActivity, SqlConnection>
    {
        Setting sett = new Setting();

        public MealActivityRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }

        public void InsertMealActivity(MealActivity mealActivity)
        {
            this.Insert(mealActivity);
        }

        public void UpdateMealActivity(MealActivity mealActivity)
        {
            this.Update(mealActivity);
        }

    }

    public class TransferRepository : BaseRepository<Transfer, SqlConnection>
    {
        Setting sett = new Setting();

        public TransferRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }

        public void InsertTransfer(Transfer transfer)
        {
            using (var connection = new SqlConnection(sett.ConString))
            {
                connection.Insert(transfer);
            }

        }

        public void UpdateTransfer(Transfer transfer)
        {
            this.Update(transfer);
        }

        public void CompleteTransaction(PaymentMain updatedPaymentMain, PaymentMain newPaymentMain, Transfer newTransfer)

        {
            using (var connection = new SqlConnection(sett.ConString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Update the existing PaymentMain
                        var paymentMainRepository = new PaymentMainRepository(sett);
                        paymentMainRepository.updatePaymentMain(updatedPaymentMain);

                        // Step 2: Insert the new PaymentMain
                        newPaymentMain.Id = 0; // Ensure it's a new record
                        paymentMainRepository.insertPaymentMain(newPaymentMain);

                        // Step 3: Insert the new Transfer
                        var transferRepository = new TransferRepository(sett);
                        transferRepository.InsertTransfer(newTransfer);

                        // Commit the transaction if everything is successful
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // Handle exceptions or errors
                        transaction.Rollback();
                        throw; // Re-throw the exception
                    }
                }
            }
        }


        public void CompleteTransaction2(PaymentMain updatedPaymentMain, PaymentMain newPaymentMain, Transfer newTransfer)
        {
            using (var connection = new SqlConnection(sett.ConString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        PaymentMainRepository paymentMainRepository = new PaymentMainRepository(sett);
                        TransferRepository transferRepository = new TransferRepository(sett);


                        // Step 2: Insert the new PaymentMain
                        paymentMainRepository.insertPaymentMain(newPaymentMain);
                        transferRepository.InsertTransfer(newTransfer);



                        updatedPaymentMain.Unit -= newPaymentMain.Unit;
                        paymentMainRepository.updatePaymentMain(updatedPaymentMain);


                        // Step 3: Update the newTransfer with the newly created PaymentMain's ID
                        // newTransfer.S_PymtMainId = updatedPaymentMain.Id;
                        // newTransfer.R_PymtMainId = newPaymentMain.Id;

                        // Commit the transaction if everything is successful
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // Handle exceptions or errors
                        transaction.Rollback();
                        throw; // Re-throw the exception
                    }
                }
            }
        }

    }




    public class MenuRepository : BaseRepository<Menu, SqlConnection>
    {
        Setting sett = new Setting();
        public MenuRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertMenu(Menu menu)
        {
            // MenuRepository menuRepository = new MenuRepository(constringmenu);
            this.Insert(menu);
        }
        public void updateMenu(Menu menu)
        {
            //MenuRepository menuRepository = new MenuRepository(constringmenu);
            this.Update(menu);
        }
        public int deleteMenu(Menu menu)
        {
            //MenuRepository menuRepository = new MenuRepository(constringmenu);
            int id = this.Delete<Menu>(menu);
            return id;
        }
        public List<Menu> GetMenus()
        {

            //    Setting sett=new Setting();
            List<Menu> menus = new List<Menu>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                menus = connection.ExecuteQuery<Menu>("[dbo].[usp_getmenus]",
                commandType: System.Data.CommandType.StoredProcedure).ToList();
                //menus = connection.QueryAll<Menu>().ToList();
                /* Do the stuffs for the people here */
            }
            return menus;
        }

        public Menu GetMenu(int id)
        {

            var menu = new Menu();
            using (var connection = new SqlConnection(sett.ConString))
            {

                menu = connection.Query<Menu>(id).FirstOrDefault();
            }
            return menu;
        }

    }
    public class MenuCategoryRepository : BaseRepository<MenuCategory, SqlConnection>
    {
        Setting sett = new Setting();
        public MenuCategoryRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertMenuCat(MenuCategory menuCat)
        {
            //MenuCategoryRepository menuCatRepository = new MenuCategoryRepository(constringmenucat);
            this.Insert(menuCat);
        }
        public void updateMenuCat(MenuCategory menuCat)
        {
            //MenuCategoryRepository menuCatRepository = new MenuCategoryRepository(constringmenucat);
            this.Update(menuCat);
        }
        public int deleteMenuCat(MenuCategory menuCat)
        {
            // MenuCategoryRepository menuCatRepository = new MenuCategoryRepository(constringmenucat);
            int id = this.Delete<MenuCategory>(menuCat);
            return id;
        }
        public List<MenuCategory> GetMenuCategories()
        {

            Setting sett = new Setting();
            List<MenuCategory> menuCategories = new List<MenuCategory>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                menuCategories = connection.QueryAll<MenuCategory>().ToList();
                /* Do the stuffs for the people here */
            }
            return menuCategories;
        }

    }

    public class PaymentDetailsRepository : BaseRepository<PaymentDetails, SqlConnection>
    {

        Setting sett = new Setting();
        public PaymentDetailsRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertPaymentDetails(PaymentDetails pymtDetails)
        {
            //PaymentDetailsRepository pymtdetailsRepository = new PaymentDetailsRepository(constringPymtDet);
            this.Insert(pymtDetails);
        }
        public void updatePaymentDetails(PaymentDetails pymtDetails)
        {
            //PaymentDetailsRepository pymtDetRepository = new PaymentDetailsRepository(constringPymtDet);
            this.Update(pymtDetails);
        }
        public int deletePymtDetails(PaymentDetails pymtDetails)
        {
            //PaymentDetailsRepository pymtDetRepository = new PaymentDetailsRepository(constringPymtDet);
            int id = this.Delete<PaymentDetails>(pymtDetails);
            return id;
        }
        public List<PaymentDetails> GetPymtDetails()
        {

            List<PaymentDetails> pymtDetails = new List<PaymentDetails>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                pymtDetails = connection.ExecuteQuery<PaymentDetails>("[dbo].[usp_getpaymentdetails]",
                commandType: System.Data.CommandType.StoredProcedure).ToList();
                /* Do the stuffs for the people here */
            }
            return pymtDetails;

        }
        public List<PaymentDetails> GetPymtDetails(int userid)
        {

            //var user=new User();
            List<PaymentDetails> paymentDetailss = new List<PaymentDetails>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                paymentDetailss = connection.ExecuteQuery<PaymentDetails>("[dbo].[usp_getpaymentdetailsbyuser]",
              new { userID = userid }, commandType: System.Data.CommandType.StoredProcedure).ToList();
                /* Do the stuffs for the people here */
            }
            return paymentDetailss;
        }

    }
    public class PaymentMainRepository : BaseRepository<PaymentMain, SqlConnection>
    {
        Setting sett = new Setting();
        public PaymentMainRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        // public int insertPaymentMain(PaymentMain pymtMain)
        // {
        //     //PaymentMainRepository pymtMainRepository = new PaymentMainRepository(constringPymtMain);
        //     int pymtMainid = Convert.ToInt16(this.Insert(pymtMain));
        //     return pymtMainid;
        // }

        public void insertPaymentMain(PaymentMain pymtMain)
        {
            using (var connection = new SqlConnection(sett.ConString))
            {
                connection.Insert(pymtMain);
            }

        }
        public void updatePaymentMain(PaymentMain pymtMain)
        {
            using (var connection = new SqlConnection(sett.ConString))
            {
                connection.Update(pymtMain);
            }

        }
        public void updatePaymentMainbyid(int opymtid, int id)
        {
            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "Update [dbo].[PaymentMain]  set opaymentid=@opaymentid where id=@id";
                connection.ExecuteNonQuery(sql, new { opaymentid = opymtid, id = @id });
            }
        }
        public int deletePymtMain(PaymentMain pymtMain)
        {
            //PaymentMainRepository pymtMainRepository = new PaymentMainRepository(constringPymtMain);

            using (var connection = new SqlConnection(sett.ConString))
            {
                var deletedrows = connection.Delete<PaymentMain>(pymtMain.Id);
                return deletedrows;
            }
        }

          public List<PaymentMain> GetPaymentbyOpaymentId(int opaymentid)
        {

            var serveds = new List<PaymentMain>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                 var sql = "SELECT * FROM Paymentmain WHERE opaymentid = @opaymentid";
                serveds = connection.ExecuteQuery<PaymentMain>(sql).ToList();
                // serveds = connection.QueryAll<Served>(e => e.isServed == 0).ToList();
              
            }
            return serveds;
        }

        public PaymentMain GetPymt(int id)
        {

            var pymt = new PaymentMain();
            using (var connection = new SqlConnection(sett.ConString))
            {

                pymt = connection.Query<PaymentMain>(id).FirstOrDefault();
            }
            return pymt;
        }
        public List<PaymentMain> GetPymtMains()
        {

            //var user=new User();
            List<PaymentMain> pymtMains = new List<PaymentMain>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                pymtMains = connection.QueryAll<PaymentMain>().ToList();
                /* Do the stuffs for the people here */
            }
            return pymtMains;
        }
        public IQueryable<PaymentByCust> GetPaidPymts()
        {

            //var user=new User();
            List<PaymentByCust> paidpymts = new List<PaymentByCust>();
            // IQueryable paidpymts;
            using (var connection = new SqlConnection(sett.ConString))
            {
                paidpymts = connection.ExecuteQuery<PaymentByCust>("[dbo].[usp_getPymtByCust]",
                commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            return paidpymts.AsQueryable();
        }
        public List<PaymentByCust> GetPaidsPymts(string CustCodeFilter)
        {
            List<PaymentByCust> paidpymts = new List<PaymentByCust>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                // var parameters = new DynamicParameters(); // Create a DynamicParameters object

                // Add the 'CustCodeFilter' parameter to the DynamicParameters
                // parameters.Add("@CustCodeFilter", CustCodeFilter);

                // Execute the stored procedure with parameters
                paidpymts = connection.ExecuteQuery<PaymentByCust>(
                    "[dbo].[usp_getPymtsByCust]",
                    new { CustCodeFilter = CustCodeFilter }, // Pass the parameters
                    commandType: System.Data.CommandType.StoredProcedure
                ).ToList();

                return paidpymts;
            }
        }


        public List<PaymentMain> GetTopNPaidPymtsForCust(string enteredby)
        {

            List<PaymentMain> paidpymts = new List<PaymentMain>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                paidpymts = connection.ExecuteQuery<PaymentMain>("[dbo].[usp_getTopNPaidPaymentsForUser]",
              new { enteredby = enteredby }, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            return paidpymts;
        }
        public List<PaymentMain> GetPaidPymtsByCust(string enteredby)
        {

            List<PaymentMain> paidpymts = new List<PaymentMain>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                paidpymts = connection.ExecuteQuery<PaymentMain>("[dbo].[usp_getPaidPymtByCust]",
              new { enteredby = enteredby }, commandType: System.Data.CommandType.StoredProcedure).ToList();
            }
            return paidpymts;
        }





    }
    public class VoucherRepository : BaseRepository<Voucher, SqlConnection>
    {
        Setting sett = new Setting();
        public VoucherRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertVoucher(Voucher voucher)
        {
            //VoucherRepository voucherRepository = new VoucherRepository(constringVoucher);
            this.Insert(voucher);
        }
        public void updateVoucher(Voucher voucher)
        {
            //VoucherRepository voucherRepository = new VoucherRepository(constringVoucher);
            this.Update(voucher);
        }
        public int deleteVoucher(Voucher voucher)
        {
            //VoucherRepository voucherRepository = new VoucherRepository(constringVoucher);
            int id = this.Delete<Voucher>(voucher);
            return id;
        }
        public List<Voucher> GetPymtVouchers()
        {

            //var user=new User();
            List<Voucher> vouchers = new List<Voucher>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                vouchers = connection.ExecuteQuery<Voucher>("[dbo].[usp_getvoucherdetails]",
               commandType: System.Data.CommandType.StoredProcedure).ToList();
                /* Do the stuffs for the people here */
            }
            return vouchers;
        }

        public List<Voucher> GetVouchers(int custTypeid)
        {

            List<Voucher> vouchers = new List<Voucher>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                // var sql = "select id,Description,Amount,Custtypeid,Active FROM [dbo].[Voucher] where isVisible=1)";
                //  vouchers=  connection.ExecuteQuery<Route>(sql).ToList(); 
                vouchers = connection.Query<Voucher>(e => e.isVisible == true).ToList();
                //    (e=>e.Custtypeid==custTypeid).ToList();
            }
            return vouchers;
        }



    }

    public class MealtariffRepository : BaseRepository<Mealtariff, SqlConnection>
    {
        Setting sett = new Setting();
        public MealtariffRepository(Setting sett) : base(sett.ConString)
        {
            this.sett = sett;
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }
        public void insertMealtariff(Mealtariff mealtariff)
        {
            //VoucherRepository voucherRepository = new VoucherRepository(constringVoucher);
            this.Insert(mealtariff);
        }
        public void updateMealtariff(Mealtariff mealtariff)
        {
            //VoucherRepository voucherRepository = new VoucherRepository(constringVoucher);
            this.Update(mealtariff);
        }
        public int deleteMealtariff(Mealtariff mealtariff)
        {
            //VoucherRepository voucherRepository = new VoucherRepository(constringVoucher);
            int id = this.Delete<Mealtariff>(mealtariff);
            return id;
        }
        public List<Mealtariff> GetMenuMealtariffs()
        {

            //var user=new User();
            List<Mealtariff> mealtariffs = new List<Mealtariff>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                mealtariffs = connection.QueryAll<Mealtariff>().ToList();
                foreach (Mealtariff mealtariff in mealtariffs)
                {
                    mealtariff.menu = connection.Query<Menu>(mealtariff.MenuId).FirstOrDefault();
                }
            }
            return mealtariffs;
        }



        public List<Mealtariff> GetMenuMealtariffby(int MealId)
        {

            List<Mealtariff> mealtariff = new List<Mealtariff>();
            using (var connection = new SqlConnection(sett.ConString))
            {
                // var sql = "select id,Description,Amount,Custtypeid,Active FROM [dbo].[Voucher] where isVisible=1)";
                //  vouchers=  connection.ExecuteQuery<Route>(sql).ToList(); 
                mealtariff = connection.Query<Mealtariff>(MealId).ToList();
                //    (e=>e.Custtypeid==custTypeid).ToList();
            }
            return mealtariff;
        }

        public Mealtariff GetMenutarrifByMaximumID(int MenuId)
        {

            var mealtarrif = new Mealtariff();
            using (var connection = new SqlConnection(sett.ConString))
            {

                mealtarrif = connection.Query<Mealtariff>(MenuId).FirstOrDefault();
            }
            return mealtarrif;
        }

        public Mealtariff GetMenutarriffByMaximumID(int MenuId)
        {
            Mealtariff mealtarrif = null;

            using (var connection = new SqlConnection(sett.ConString))
            {

                var sql = "SELECT TOP 1 * FROM Mealtariff WHERE MenuId = @MenuId ORDER BY Id DESC";
                mealtarrif = connection.ExecuteQuery<Mealtariff>(sql, new { MenuId = MenuId }).FirstOrDefault();

                if (mealtarrif != null)
                {
                    mealtarrif.menu = connection.Query<Menu>(mealtarrif.MenuId).FirstOrDefault();
                }
            }

            return mealtarrif;
        }

        public List<Menuandtariff> GetMenuandtariff()
        {

            var menuandtariff = new List<Menuandtariff>();

            using (var connection = new SqlConnection(sett.ConString))
            {
                var sql = "WITH CTE AS (SELECT m.name AS MenuName, m.id AS menuid,m.active as activestatus, mt.tariff AS Tariff, ROW_NUMBER() OVER (PARTITION BY m.id ORDER BY mt.id DESC) AS RowNum FROM menu AS m INNER JOIN mealtariff AS mt ON m.id = mt.menuid ) SELECT MenuName, menuid,activestatus, Tariff FROM CTE WHERE RowNum = 1  AND activestatus = 1";
                menuandtariff = connection.ExecuteQuery<Menuandtariff>(sql).ToList();
                foreach (Menuandtariff mt in menuandtariff)
                {
                    mt.menu = connection.Query<Menu>(mt.MenuId).FirstOrDefault();
                }
            }
            return menuandtariff;
        }


    }

    public class FeedbackRepository : BaseRepository<Feedback, SqlConnection>
    {
        public FeedbackRepository(Setting sett) : base(sett.ConString)
        {
            DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
            DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
            StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(new SqlServerDbSetting()), true);
        }

        public void InsertFeedback(Feedback feedback)
        {
            this.Insert(feedback);
        }
    }
}




