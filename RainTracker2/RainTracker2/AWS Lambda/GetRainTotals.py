
import requests
import mysql.connector
import uuid
import json
from datetime import datetime

# Triggered daily by AWS CloudWatch
def lambda_handler(event, context):
    
    global mydb
    try:
        mydb = mysql.connector.connect(
            host= "database-2.xxxxxxxxxxxxxxxx.amazonaws.com",
            user="xxxx",
            passwd="xxxx",
            database="xxxx",
            port= 3306
    )
    #print("Database connected")
    except:
        print("Database not connecting")
        sys.exit()

    sql = "SELECT locationkey FROM Locations";
    mycursor = mydb.cursor()
    mycursor.execute(sql)
    keys = []
    for row in mycursor.fetchall():
        keys.append(row[0])

    SaveTotals(keys, mycursor)

def SaveTotals(keys, mycursor):
# Get 24hr rain data from Accuweather for each location key    
    for key in keys:
        apistring = "http://dataservice.accuweather.com/currentconditions/v1/" + key + "?apikey=xxxxxxxxxxxxxxxxxx&details=true"
        response = requests.get(apistring)
        content = response.content.decode('UTF-8')
        jsArray = json.loads(content)
        jsObj = jsArray[0]
        print(json.dumps(jsObj, indent = 4, sort_keys=True))

        inches = jsObj["PrecipitationSummary"]["Past24Hours"]["Imperial"]["Value"]
        guid = str(uuid.uuid1().hex)
        sql = "INSERT into RainfallTotals (guid, locationkey, date, inches) VALUES (%s, %s, now(), %s)"
        err = mycursor.execute(sql, (guid, key, inches))
        mydb.commit()
    
    mycursor.close()
    mydb.close()