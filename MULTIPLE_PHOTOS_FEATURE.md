# Функциональность: Несколько фотографий для одной сцены

## Что было добавлено

Теперь можно добавлять **несколько фотографий** для одной сцены и переключаться между ними в VR.

---

## Изменения в базе данных

### Новая таблица `scene_photos`

```sql
CREATE TABLE scene_photos (
    id INTEGER PRIMARY KEY,
    create_date DATETIME,
    update_date DATETIME,
    scene_id INTEGER REFERENCES scenes(id),
    filename VARCHAR(256),
    order_index INTEGER DEFAULT 0
)
```

**Миграция**: `Migration_202602111700` автоматически перенесла существующие фотографии из таблицы `scenes` в `scene_photos`.

---

## Backend API

### Новые endpoints

1. **Добавить фото к сцене**
   ```
   POST /api/scenes/{sceneId}/photos
   Content-Type: multipart/form-data
   Authorization: Bearer <token>
   
   Body: formFile (файл изображения)
   ```

2. **Получить список фотографий сцены**
   ```
   GET /api/scenes/{sceneId}/photos
   Authorization: Bearer <token>
   
   Response: ScenePhotoDto[]
   ```

3. **Получить фото по индексу (для VR)**
   ```
   GET /api/scenes/photos/by-index/{photoIndex}
   Header: MacAddress
   
   Response: image file
   ```

---

## Web UI (Blazor)

### Как использовать

1. Перейдите на страницу `/scenes`
2. Создайте новую сцену (загружается первое фото)
3. Для добавления дополнительных фото:
   - Нажмите кнопку **"Добавить фото"** на карточке сцены
   - Выберите файл изображения
   - Нажмите **"Добавить"**

Теперь на карточке сцены отображается количество фотографий: `Название сцены (3 фото)`

---

## Unity VR

### Как использовать в VR

1. **Автоматическая загрузка всех фото**
   - При входе в сцену загружаются все фотографии (до 20 штук)
   - Первая фотография (индекс 0) отображается автоматически

2. **Переключение между фотографиями (Meta Quest 3 / 3S)**
   - Компонент `PhotoSwitcher` автоматически добавляется на Tablet
   - **Левый контроллер:** кнопка **Y** — предыдущая панорама; **Grip** — следующая
   - **Левый стик** влево/вправо — альтернативное переключение
   - Переключение зациклено (после последнего → первое)

3. **Отображение счётчика**
   - Добавьте `TMP_Text` компонент для отображения: "Фото 2 из 5"

### Пример настройки в Unity

```csharp
// В сцене CrimePhoto добавьте объект с компонентом PhotoSwitcher

public class PhotoSwitcher : MonoBehaviour
{
    [SerializeField] private TMP_Text _photoCounterText;
    
    public void NextPhoto() { ... }
    public void PreviousPhoto() { ... }
}
```

**UI кнопки**:
- Левая кнопка → `PhotoSwitcher.PreviousPhoto()`
- Правая кнопка → `PhotoSwitcher.NextPhoto()`

---

## Технические детали

### CurrentState (Unity)

Добавлены новые поля:

```csharp
public static List<Texture2D> ScenePhotos { get; set; }     // Все загруженные фото
public static int CurrentPhotoIndex { get; set; }           // Текущий индекс
public static int TotalPhotos { get; set; }                 // Общее количество
```

### Загрузка фото

Фотографии загружаются последовательно при логине:
- Индекс 0, 1, 2, ... до тех пор, пока сервер возвращает 404

### Skybox

При переключении фото обновляется skybox материал:

```csharp
RenderSettings.skybox = new Material(Shader.Find("Skybox/Panoramic")) {
    mainTexture = CurrentState.ScenePhotos[CurrentState.CurrentPhotoIndex]
};
DynamicGI.UpdateEnvironment();
```

---

## Запуск после обновления

1. **Остановите сервер** (если запущен)
2. **Запустите миграцию**:
   ```powershell
   cd e:\diploma_maga\1\crime-scene-service-main\src\CSService.Migrations
   dotnet run
   ```
3. **Запустите сервер**:
   ```powershell
   cd e:\diploma_maga\1\crime-scene-service-main\src\CSService.API
   dotnet run
   ```
4. **В Unity**:
   - Откройте сцену `CrimePhoto`
   - Добавьте GameObject с компонентом `PhotoSwitcher`
   - Настройте UI кнопки для переключения

---

## Примечания

- Максимальное количество фотографий на сцену: **20** (можно изменить в `Login.cs`)
- Фотографии загружаются один раз при входе в сцену
- Все фотографии хранятся в памяти (учитывайте размер текстур для VR)
- Порядок фотографий определяется полем `order_index` в БД

---

Готово! Теперь можно работать с несколькими фотографиями сцены. 🎉
