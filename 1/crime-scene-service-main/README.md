# crime-scene-service
## Сощдание Docker image
Что бы создать docker image нужно ввести следующую команду в терминал находясь в корне проекта:
```shell
docker build -t crime-service-image -f Dockerfile .
```

Для запуска контейнера по созданному docker image нужно ввести следующую команду:
```shell
docker run -d -p 5197:8080 --name <container-name> crime-service-image
```

## Регистрация
Для регистрации нужно ввести в поле для поиска в браузере следующий адрес и пройти процесс регистрации:
```
http://<server-ip>:5197/register
```