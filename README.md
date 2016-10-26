# MongoLogger
A log library for c#, Quick and simple log any object to Mongodb, support colud mlab mognodb server.

# Nuget
Install-Package MongoLogger

#Source
https://github.com/rolence0515/MongoLogger/tree/master/ConsoleApplication1/MongoLogger

#How to use
            //init
		          MongoLogger.MongoLog.Init(
                new MongoLog.AppOption() { AuthorName = "YOUR_NAME", ProjectName = "YOUR_PROJECTNAME" },
                new MongoLog.MailNotifyOption()
                {
                    MailList = new string[] { "YOUR_RECEIVER_EMAIL" }, 
                    MailServer = "smtp.mailgun.org", //if you use mailgun as mail server
                    MailSubjectTemplate = "{0}-Error", //title template
                    NetworkCredential = new System.Net.NetworkCredential() { UserName = "YOURNAME", Password = "PASSWORD" },
                    LogLevel = Serilog.Events.LogEventLevel.Error,
                    BatchPostingLimit = 10
                },
                new MongoLog.MongoOption()
                {
                    MongoDBIp = "YOUR_MONGODB_IP",
                    CollectionName = "Log",
                    CollectionMB = 5,
                    LogLevel = Serilog.Events.LogEventLevel.Error,
                    BatchPostingLimit = 5,
                    BatchTimePostingLimit = new TimeSpan(0, 0, 5)
                }
            );
		
		
			      //log
            Log.Information("This is test log:{@app},{count}", new { author = "rolence" }, 1);
            Log.Error(new Exception("Test exception"), "This is test error log:{@app},{count}", new { author = "rolence" }, 1);
            
  
#Feature
1.User LogLevel to setting what level you want to log to.  
2.Not only silent log info to mongodb, but also supprt mail notify  
3.BatchPostingLimit and BatchTimePostingLimit supprt log buffer, prevent write too often.  
4.Mongodb capped collection support, cycling uses collection space, the limitation is CollectionMB.  
4.Eazy to use.

#Reference
https://serilog.net/
