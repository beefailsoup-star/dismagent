class Memory:
    def __init__(self):
        self.history = []

    def add(self, user_input, response):
        self.history.append({"input": user_input, "response": response})

    def get_last(self, n=5):
        return self.history[-n:]
