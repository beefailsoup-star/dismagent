from config_manager import ConfigManager
from memory import Memory
from tools import Tools
from task_parser import TaskParser

class Agent:
    def __init__(self):
        self.config = ConfigManager()
        self.memory = Memory()
        self.tools = Tools()
        self.parser = TaskParser()

    def handle(self, user_input):
        task = self.parser.parse(user_input)

        if task["type"] == "search":
            result = self.tools.search_web(task["query"])
        elif task["type"] == "email":
            result = self.tools.send_email(task["to"], task["subject"], task["body"])
        else:
            result = f"回覆: {task['text']}"

        self.memory.add(user_input, result)
        return result
