# crime-scene-service — VR-тренажёр осмотра места происшествия

## Быстрый старт (Windows)

```powershell
cd e:\diploma_maga\1\crime-scene-service-main
.\setup-vosk.ps1      # один раз: модель распознавания речи
.\setup-and-run.ps1   # миграции БД + запуск API :5197
```

Регистрация преподавателя: http://127.0.0.1:5197/register  
VR-шлем: Meta Quest 3 / 3S, Unity-проект: `e:\diploma_maga\2\vr-crime-scene-main`  
Подробная инструкция Unity: `e:\diploma_maga\ИНСТРУКЦИЯ UNITY.txt`

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