import json  # Для работы с JSON
import tkinter as tk  # Для создания графического интерфейса
from tkinter import messagebox  # Для вывода сообщений
# 1. Observable: Реализация паттерна "Наблюдатель"
class Observable:
    def __init__(self):
        self._observers = []  # Список наблюдателей, подписанных на изменения
    def add_observer(self, observer):
        """Добавить наблюдателя."""
        self._observers.append(observer)  # Добавляем наблюдателя в список
    def notify_observers(self):
        """Уведомить всех наблюдателей об изменении данных."""
        for observer in self._observers:  # Обходим всех наблюдателей
            observer()  # Вызываем метод обновления у каждого
# 2. BaseRepository: Базовый класс для работы с данными
class BaseRepository(Observable):
    def __init__(self, file_path):
        super().__init__()  # Наследуем функционал Observable
        self.file_path = file_path  # Путь к файлу данных
        self.data = []  # Список данных
        self._load_data()  # Загружаем данные из файла при инициализации
    def _load_data(self):
        """Метод чтения данных. Реализуется в наследниках."""
        raise NotImplementedError
    def _save_data(self):
        """Метод записи данных. Реализуется в наследниках."""
        raise NotImplementedError
    def get_all(self):
        """Получить все данные."""
        return self.data  # Возвращаем список всех данных
    def add(self, obj):
        """Добавить новый объект в список."""
        max_id = max((item['id'] for item in self.data), default=0)  # Вычисляем максимальный ID
        obj['id'] = max_id + 1  # Присваиваем новый уникальный ID
        self.data.append(obj)  # Добавляем объект в список
        self._save_data()  # Сохраняем данные в файл
        self.notify_observers()  # Уведомляем всех наблюдателей об изменении
    def delete_by_id(self, obj_id):
        """Удалить объект по ID."""
        self.data = [item for item in self.data if item['id'] != obj_id]  # Удаляем объект с указанным ID
        self._save_data()  # Сохраняем изменения в файл
        self.notify_observers()  # Уведомляем наблюдателей об изменении данных
# 3. JSONRepository: Репозиторий для работы с JSON
class JSONRepository(BaseRepository):
    def _load_data(self):
        """Чтение данных из JSON-файла."""
        try:
            with open(self.file_path, 'r', encoding='utf-8') as file:
                self.data = json.load(file)  # Загружаем данные из JSON
        except FileNotFoundError:
            self.data = []  # Если файл отсутствует, инициализируем пустой список
    def _save_data(self):
        """Сохранение данных в JSON-файл."""
        with open(self.file_path, 'w', encoding='utf-8') as file:
            json.dump(self.data, file, indent=4, ensure_ascii=False)  # Сохраняем данные в JSON
# 4. Controller: Управляет репозиторием и представлением
class Controller:
    def __init__(self, repository, view):
        self.repository = repository  # Ссылка на репозиторий данных
        self.view = view  # Ссылка на главное окно
        self.repository.add_observer(self.update_view)  # Подписываем окно на изменения в репозитории
    def update_view(self):
        """Обновить данные в главном окне."""
        self.view.display_data(self.repository.get_all())  # Передаём данные из репозитория в главное окно
    def add_item(self, item_data):
        """Добавить элемент в репозиторий."""
        self.repository.add(item_data)  # Добавляем данные в репозиторий
    def delete_item(self, item_id):
        """Удалить элемент из репозитория по ID."""
        self.repository.delete_by_id(item_id)  # Удаляем данные из репозитория
# 5. ClientView: Главное окно приложения
class ClientView:
    def __init__(self, controller):
        self.controller = controller  # Ссылка на контроллер
        self.root = tk.Tk()  # Создаем главное окно
        self.root.title("Client Manager")  # Устанавливаем заголовок окна
        # Список клиентов
        self.data_list = tk.Listbox(self.root)  # Виджет для отображения списка клиентов
        self.data_list.
pack(fill=tk.BOTH, expand=True)  # Размещаем список в окне
        # Кнопки управления
        tk.Button(self.root, text="Add Client", command=self.add_client).pack(side=tk.LEFT)  # Кнопка добавления
        tk.Button(self.root, text="Delete Client", command=self.delete_client).pack(side=tk.LEFT)  # Кнопка удаления
    def display_data(self, data):
        """Отобразить данные в списке."""
        self.data_list.delete(0, tk.END)  # Очищаем список
        for item in data:  # Проходим по всем элементам данных
            self.data_list.insert(tk.END, f"{item['id']}: {item['first_name']} {item['last_name']}")  # Добавляем данные в список
    def add_client(self):
        """Добавить клиента (имитация)."""
        new_client = {"first_name": "Новый", "last_name": "Клиент", "email": "example@example.com"}  # Пример данных клиента
        self.controller.add_item(new_client)  # Передаём данные контроллеру для добавления
    def delete_client(self):
        """Удалить выбранного клиента."""
        selected = self.data_list.curselection()  # Получаем выбранный элемент списка
        if not selected:  # Если элемент не выбран
            messagebox.showwarning("Warning", "Please select a client to delete.")  # Показываем предупреждение
            return
        client_id = int(self.data_list.get(selected[0]).split(":")[0])  # Извлекаем ID клиента из строки
        self.controller.delete_item(client_id)  # Передаём ID контроллеру для удаления
    def run(self):
        """Запустить главное окно."""
        self.root.mainloop()  # Запускаем цикл обработки событий
# 6. Инициализация приложения
if name == "__main__":
    # Создаем репозиторий для хранения данных
    repo = JSONRepository("clients.json")  # Репозиторий работает с файлом JSON
    # Создаем главное окно и контроллер
    view = ClientView(Controller(repo, None))  # Создаем главное окно и связываем его с контроллером
    view.controller.view = view  # Устанавливаем ссылку на главное окно в контроллере
    # Запускаем приложение
    view.run()  # Запускаем главное окно
