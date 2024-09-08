<template>
  <el-container class="chat-container">
    <el-main class="chat-messages">
      <div
        v-for="message in chatMessages"
        :key="message.chattime"
        :class="['message', isMyMessage(message) ? 'my-message' : 'other-message']"
      >
        <el-card shadow="never" class="message-card">
          <el-row type="flex" justify="space-between">
            <el-col>
              <strong v-if="!isMyMessage(message)">匿名用户{{ message.userId }}</strong>
            </el-col>
            <el-col>
              <small>{{ formatTime(message.chattime) }}</small>
            </el-col>
          </el-row>
          <p>{{ message.content }}</p>
        </el-card>
      </div>
    </el-main>
    <el-footer class="chat-input">
      <el-input
        v-model="newMessage"
        placeholder="输入消息..."
        @keyup.enter="sendMessage"
        class="input"
      ></el-input>
      <el-button type="primary" @click="sendMessage">发送</el-button>
    </el-footer>
  </el-container>
</template>

<script>
import axios from 'axios';
import { ElMessage } from 'element-plus';
const BACKEND_BASE_URL = import.meta.env.VITE_API_BACKEND_BASE_URL;

export default {
  data() {
    return {
      chatMessages: [], // 聊天记录
      newMessage: '', // 新消息内容
      userid: localStorage.getItem('userId') || '', // 用户id
    };
  },
  mounted() {
    this.getChatMessages(10); // 默认获取最近10条消息
  },
  methods: {
    // 获取聊天记录
    async getChatMessages(n) {
      try {
        const response = await axios.get(`${BACKEND_BASE_URL}/chat?n=${n}`);
        console.log('获取的聊天记录:', response.data); // 调试用，输出聊天记录
        this.chatMessages = response.data.map((msg) => ({
          ...msg,
          userId: msg.userid || '', // 给消息添加 userId，如果没有则默认
        }));
      } catch (error) {
        console.error('获取聊天记录失败', error);
        ElMessage.error('获取聊天记录失败');
      }
    },
    // 发送消息
    async sendMessage() {
      if (this.newMessage.trim() === '') return; // 确保消息不为空

      const messageData = {
        userId: this.userid,
        chattime: new Date().toISOString(), // 发送的时间
        content: this.newMessage, // 发送的消息内容
      };

      try {
        const response = await axios.post(`${BACKEND_BASE_URL}/chat`, messageData);
        console.log('发送消息成功:', response.data); // 调试用，输出发送消息的返回结果

        // 手动添加新消息到chatMessages数组，避免等待网络延迟
        this.chatMessages.push(messageData);
        this.newMessage = ''; // 清空输入框
      } catch (error) {
        console.error('发送消息失败', error);
        ElMessage.error('发送消息失败');
      }
    },
    // 格式化时间
    formatTime(time) {
      const date = new Date(time);
      return `${date.getHours()}:${date.getMinutes().toString().padStart(2, '0')}`;
    },
    // 判断是否是自己的消息
    isMyMessage(message) {
      return message.userId === this.userid;
    },
  },
};
</script>

<style scoped>
.chat-container {
  display: flex;
  flex-direction: column;
  height: 90vh; /* 调整为 90vh 以确保输入框可见 */
  justify-content: space-between;
  background-color: #f0f0f5;
}

.chat-messages {
  flex-grow: 1;
  overflow-y: auto;
  padding: 5px 10px; /* 减少上下的内边距 */
  border: 1px solid #ddd;
  background-color: #fafafa;
}

.message {
  display: flex;
  margin-bottom: 5px; /* 缩小消息之间的间距 */
}

.my-message {
  justify-content: flex-end;
}

.other-message {
  justify-content: flex-start;
}

.message-card {
  max-width: 50%; /* 缩小消息气泡的宽度 */
  padding: 8px 12px; /* 调整气泡的内边距，使其更紧凑 */
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.my-message .message-card {
  background-color: #d1e7dd;
  border-top-right-radius: 0;
}

.other-message .message-card {
  background-color: #e9ecef;
  border-top-left-radius: 0;
}

.chat-input {
  display: flex;
  padding: 8px; /* 调整输入框区域的内边距 */
  border-top: 1px solid #ccc;
  background-color: #ffffff;
}

.input {
  flex-grow: 1;
  margin-right: 8px; /* 调整输入框与发送按钮之间的间距 */
}

</style>
