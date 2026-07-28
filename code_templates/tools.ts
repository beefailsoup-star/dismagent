// Tools.ts
export class Tools {
  searchWeb(query: string): string {
    // 這裡可以接 API，例如 fetch 或 axios
    return `搜尋結果: ${query}`;
  }

  sendEmail(to: string, subject: string, body: string): string {
    // 這裡可以接 SMTP 或第三方 API
    return `寄信給 ${to}, 主題: ${subject}, 內容: ${body}`;
  }
}
