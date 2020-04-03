front=false;back=false;(cd C:/Users/Juulia/arttu/pesis/pesisBack &&
docker build . -t back &&
(heroku container:push web --app pesisback ||
heroku container:release web --app pesisback) &&
back=true || back=false);(cd C:/Users/Juulia/arttu/pesis/pesisFront &&
ng build --prod --output-path=dist &&
docker build . -t front &&
(heroku container:push web --app pesisstats ||
heroku container:release web --app pesisstats) &&
(front=true; echo "true") || (front=false; echo "false"));echo "";echo "";if $back
then
    echo "Deploying pesisBack succesful"
else
   echo "FAILED: Deploying pesisBack FAILED"
fi; if $front
then
    echo "Deploying pesisFront succesful"
else
   echo "FAILED: Deploying pesisFront FAILED"
fi;

