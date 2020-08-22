
import requests
import json
import mysql.connector
import uuid
from datetime import datetime

def lambda_handler(event, context):
  
    dict = json.loads(event['body'])     
    print(dict)    
    method = dict['function']
    result = ''

    ConnectToDB()

    if method == 'getlocations': 
        print(method)
        result = GetLocations()
    elif method == 'addlocation':
        result = AddLocation()
    elif method == 'gettotals':
        result = GetRainfallTotals()
    else:
        result = 'function not recognized'
    
    responseObject = {}
    responseObject['statusCode'] = 200
    responseObject['headers'] = {}
    responseObject['headers']['Content-Type'] = 'application/json'
    responseObject['body'] = json.dumps(result)
    return responseObject 


def ConnectToDB():
    global mydb
    try:
        mydb = mysql.connector.connect(
            host= "database-2.xxxxxxxxxxxxxxxx.amazonaws.com",
            user="xxxx",
            passwd="xxxx",
            database="xxxx",
            port= 3306
        )
        print('Database connected')
    except:
        print('Database not connecting')
        sys.exit()

def packageData(cursor):
    # iterate through cursor and build list of dictionary
    
    desc = cursor.description
    column_names = [col[0] for col in desc]
    data = [dict(zip(column_names, row))  
        for row in cursor.fetchall()]
    
    package = json.dumps(data, indent=4, sort_keys=False, default=str)
    return package

def AddLocation(latitude, longitude):
    
    print(latitude)
    print(longitude)
    apistring = "http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?apikey=xxxxxxxxxxxxxxxxxxxxx&q=" + latitude + "%2C%20" + longitude
    response = requests.get(apistring)
    content = response.content.decode('UTF-8')
    print(content)    
    jsObj = json.loads(content)

    locationKey = jsObj["Key"]
    locName = jsObj["LocalizedName"]
    apilat = jsObj["GeoPosition"]["Latitude"]
    apilong = jsObj["GeoPosition"]["Longitude"]

    print(locationKey)
    
    mycursor = mydb.cursor()
    sql = "INSERT INTO Locations (locationkey, locationname, latitude, longitude) VALUES (%s, %s, %s, %s);"
    mycursor.execute(sql, (locationKey, locName, apilat, apilong))
    mycursor.close()
    mydb.commit()

    return True

def GetRainfallTotals():

    sql = "SELECT * FROM RainfallTotals";
    mycursor = mydb.cursor()
    mycursor.execute(sql)
    return packageData(mycursor)
    

def GetLocations():

    sql = "SELECT * from Locations";
    mycursor = mydb.cursor()
    mycursor.execute(sql)
    return packageData(mycursor)
    

