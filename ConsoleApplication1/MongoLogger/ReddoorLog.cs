using Serilog;
using System;
using System.Net;

namespace MongoLogger
{
    public class MongoLog
    {
        public class AppOption
        {
            public string ProjectName { get; set; }

            public string AuthorName { get; set; }
        }

        public class MailNotifyOption
        {
            public string[] MailList { get; set; }

            public int BatchPostingLimit { get; set; }

            public Serilog.Events.LogEventLevel LogLevel { get; set; }

            public string MailServer { get; set; }

            public NetworkCredential NetworkCredential { get; set; }

            public string MailSubjectTemplate { get; set; }
        }

        public class MongoOption
        {
            public readonly string MongodbAuthor_Project;

            public string MongoDBIp { get; set; }

            public string CollectionName { get; set; }

            public int CollectionMB { get; set; }

            public int BatchPostingLimit { get; set; }

            public TimeSpan BatchTimePostingLimit { get; set; }

            public Serilog.Events.LogEventLevel LogLevel { get; set; }
        }

        public class MongoOptionV2
        {
            public readonly string MongodbAuthor_Project;

            public string DatabaseUrl { get; set; }

            public string CollectionName { get; set; }

            public int CollectionMB { get; set; }

            public int BatchPostingLimit { get; set; }

            public TimeSpan BatchTimePostingLimit { get; set; }

            public Serilog.Events.LogEventLevel LogLevel { get; set; }
        }

        /// <summary>
        /// 初始化並建立Logger
        /// </summary>
        public static void Init
       (
           AppOption app,
           MailNotifyOption mail,
           MongoOption mo
       )
        {
            var mov2 = new MongoOptionV2()
            {
                BatchPostingLimit = mo.BatchPostingLimit,
                BatchTimePostingLimit = mo.BatchTimePostingLimit,
                CollectionMB = mo.CollectionMB,
                CollectionName = mo.CollectionName,
                LogLevel = mo.LogLevel,
                DatabaseUrl = string.Format("mongodb://{0}:{1}/{2}", mo.MongoDBIp, "27017", app.AuthorName + "_" + app.ProjectName)
            };
            Log.Logger = DefaultWriteTo(app, mail, mov2).CreateLogger();
        }

        /// <summary>
        /// 初始化並建立Logger
        /// </summary>
        public static void Init
       (
           AppOption app,
           MailNotifyOption mail,
           MongoOptionV2 mo
       )
        {
            Log.Logger = DefaultWriteTo(app, mail, mo).CreateLogger();
        }

        /// <summary>
        /// 取得預設的初始化設定WriteTo
        /// </summary>
        public static LoggerConfiguration DefaultWriteTo
        (
            AppOption app,
            MailNotifyOption mail,
            MongoOptionV2 mo
        )
        {
            string author = app.AuthorName;

            var writeto = new LoggerConfiguration().MinimumLevel.Verbose();

            //ColoredConsole
            writeto = writeto.WriteTo.LiterateConsole(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose);

            //SlowNotify
            if (mail != null)
            {
                writeto = writeto.WriteTo.Email(
                    fromEmail: mail.MailList[0],
                    toEmails: mail.MailList,
                    mailServer: mail.MailServer,// "smtp.mailgun.org",
                    networkCredential: mail.NetworkCredential, //new System.Net.NetworkCredential("postmaster@sandbox265b75940b0f48f38cce4c85ee27f792.mailgun.org", "test123456"),
                    mailSubject: string.Format(mail.MailSubjectTemplate /*"【{0}】出問題了，請檢查"*/, app.ProjectName),
                    restrictedToMinimumLevel: mail.LogLevel,
                    batchPostingLimit: mail.BatchPostingLimit
                );
            }

            //MongoDBCapped
            if (mo != null)
            {
                writeto = writeto.WriteTo.MongoDBCapped(
                    databaseUrl: mo.DatabaseUrl,
                    restrictedToMinimumLevel: mo.LogLevel,
                    cappedMaxSizeMb: mo.CollectionMB,
                    cappedMaxDocuments: null,
                    collectionName: mo.CollectionName,
                    batchPostingLimit: mo.BatchPostingLimit,
                    period: mo.BatchTimePostingLimit,
                    formatProvider: null);
            }

            writeto = writeto.Enrich.WithProperty("dApp", app.ProjectName).Enrich.WithProperty("dAuthor", app.AuthorName); //因為可能公用，所以自動加上dApp維度

            return writeto;
        }

        public static LoggerConfiguration DefaultWriteTo
        (
            AppOption app,
            MailNotifyOption mail,
            MongoOption mo
        )
        {
            var mov2 = new MongoOptionV2()
            {
                BatchPostingLimit = mo.BatchPostingLimit,
                BatchTimePostingLimit = mo.BatchTimePostingLimit,
                CollectionMB = mo.CollectionMB,
                CollectionName = mo.CollectionName,
                LogLevel = mo.LogLevel,
                DatabaseUrl = string.Format("mongodb://{0}:{1}/{2}", mo.MongoDBIp, "27017", app.AuthorName + "_" + app.ProjectName)
            };
            return DefaultWriteTo(app, mail, mov2);
        }
    }
}