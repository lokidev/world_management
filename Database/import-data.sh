#run the setup script to create the DB and the schema in the DB
#do this in a loop because the timing for when the SQL instance is ready is indeterminate
for i in {1..50}; do
    /opt/mssql-tools/bin/sqlcmd -S world-management-db -U sa -P Yukon900 -d master -i setup.sql
    if [ $? -eq 0 ]; then
        echo "setup.sql completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done

#import the data from the csv file
#/opt/mssql-tools/bin/bcp World.dbo.World in "/usr/src/app/World.csv" -c -t',' -S world-management-db -U sa -P Yukon900
