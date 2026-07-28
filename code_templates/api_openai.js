import fetch from "node-fetch";

async function callOpenAI(prompt) {
  const response = await fetch("http://localhost:8000/v1/chat/completions", {
    method: "POST",
    headers: {
      "Authorization": `Bearer YOUR_API_KEY`,
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      model: "gpt-3.5-turbo", // 或相容模型
      messages: [{ role: "user", content: prompt }]
    })
  });

  const data = await response.json();
  console.log(data.choices[0].message.content);
}

callOpenAI("你好，幫我生成一個範例訊息");
