cd C:/Users/Juulia/arttu/pesis/pesisBack
docker build . -t back
heroku container:push web --app pesisback
heroku container:release web --app pesisback

cd C:/Users/Juulia/arttu/pesis/pesisFront
docker build . -f ./Dockerfile-build/ -t front
heroku container:push web --app pesisstats
heroku container:release web --app pesisstats