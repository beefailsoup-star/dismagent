class TaskParser:
    def parse(self, text):
        if "搜尋" in text:
            return {"type": "search", "query": text.replace("搜尋", "").strip()}
        elif "寄信" in text:
            return {"type": "email", "to": "test@example.com", "subject": "測試", "body": text}
        else:
            return {"type": "chat", "text": text}
