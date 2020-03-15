cd C:/Users/Juulia/arttu/pesis/pesisBack &&
docker build . -t back &&
heroku container:push web --app shrouded-savannah-06829 &&
heroku container:release web --app shrouded-savannah-06829 &&
cd C:/Users/Juulia/arttu/pesis/pesisFront &&
ng build --prod --output-path=dist &&
docker build . -t front &&
heroku container:push web --app young-beyond-12566 &&
heroku container:release web --app young-beyond-12566 &&
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
