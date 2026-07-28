<template>
  <div class="chat-container">
    <h2>My Agent</h2>
    <div class="messages">
      <div v-for="(msg, idx) in messages" :key="idx" :class="msg.role">
        <strong>{{ msg.role }}:</strong> {{ msg.text }}
      </div>
    </div>
    <div class="input-area">
      <input v-model="userInput" @keyup.enter="sendMessage" placeholder="輸入訊息..." />
      <button @click="sendMessage">送出</button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const messages = ref([
  { role: 'agent', text: '你好，我是你的 Agent！' }
])
const userInput = ref('')

function sendMessage() {
  if (!userInput.value) return
  messages.value.push({ role: 'user', text: userInput.value })
  // 模擬 Agent 回覆
  messages.value.push({ role: 'agent', text: `我收到: ${userInput.value}` })
  userInput.value = ''
}
</script>

<style scoped>
.chat-container { max-width: 400px; margin: auto; border: 1px solid #ccc; padding: 1rem; }
.messages { min-height: 200px; margin-bottom: 1rem; }
.user { color: blue; }
.agent { color: green; }
.input-area { display: flex; gap: 0.5rem; }
</style>
