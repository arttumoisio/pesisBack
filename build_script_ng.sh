cd C:/Users/Juulia/arttu/pesis/pesisBack &&
docker build . -t back &&
heroku container:push web --app pesisback &&
heroku container:release web --app pesisback &&

cd C:/Users/Juulia/arttu/pesis/pesisFront &&
ng build --prod --output-path=dist &&
docker build . -t front &&
heroku container:push web --app pesisstats &&
heroku container:release web --app pesisstats &&

echo '
DEPLOYMENT WAS SUCCESFUL!
DEPLOYMENT WAS SUCCESFUL!
'|| 
echo '
DEPLOYMENT FAILED!!
DEPLOYMENT FAILED!!
DEPLOYMENT FAILED!!
DEPLOYMENT FAILED!!
DEPLOYMENT FAILED!!
'
