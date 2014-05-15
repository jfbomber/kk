echo 'Coping web config...' 


# PATH='C:\Users\Administrator\dropbox\sites\export\kenya_keys\kenya_keys\kenya_keys'

FILE_PATH='C:\Users\Administrator\dropbox\sites\export\kenya_keys\kenya_keys\kenya_keys\Web.config'
TEMP_PATH='C:\Users\Administrator\dropbox\sites\export\kenya_keys\kenya_keys\kenya_keys\Temp.config'
echo 'Removing line from temp web config...'


sed 's/<add assembly="Mono.Data.Sqlite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756" \/>/ /g' $FILE_PATH > $TEMP_PATH


#cd /Users/jfoster/Dropbox/FWM/Sites

# cd C:\Users\Administrator\Dropbox\Sites\export/Ken

# FILE_PATH='/Users/jfoster/Dropbox/FWM/Sites/kenya_keys/kenya_keys/Web.config'

# TEMP_PATH='/Users/jfoster/Dropbox/FWM/Sites/kenya_keys/kenya_keys/Temp.config'





#### REMOVE THE TEMP FILE ####
# rm $FILE_PATH
# cp -P $TEMP_PATH $FILE_PATH
# rm $TEMP_PATH


# copy dlls

# copy database


# remove clear files in old site folder


# copy the temp folder over 

# copy remove temp folder






#### text to replace ####

# s/kenya_keys/sqlite3.dll

# 

# FILE_PATH='C:\Users\Administrator\Dropbox\Sites\export\kenya_keys\kenya_keys\Web.config'
# TEMP_PATH='C:\Users\Administrator\Dropbox\Sites\export\kenya_keys\kenya_keys\Temp.config'






# Copy the files into the bin
#cp -P "$ITUNES_APP_PATH" "$APP_PATH"



# Copy database from old folder and put it into the new one


# delete files from the old server