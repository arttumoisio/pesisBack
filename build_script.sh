cd C:/Users/Juulia/arttu/pesis/pesisBack
docker build . -t back
heroku container:push web --app shrouded-savannah-06829
heroku container:release web --app shrouded-savannah-06829

cd C:/Users/Juulia/arttu/pesis/pesisFront
docker build . -f ./Dockerfile-build/ -t front
heroku container:push web --app young-beyond-12566
heroku container:release web --app young-beyond-12566