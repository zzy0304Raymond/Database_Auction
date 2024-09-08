<template>
    <div class="payment-container">
      <!-- 打印 paymentRecords 以确认数据传递 -->
      <pre>{{ paymentRecords }}</pre>
  
      <div v-if="user && paymentRecords.length">
        <!-- 用户信息部分 -->
        <div class="user-info-card">
          <h2>当前用户</h2>
          <p><strong>用户名:</strong> {{ user.username }}</p>
          <p><strong>邮箱:</strong> {{ user.email }}</p>
        </div>
  
        <!-- 支付记录部分 -->
        <div class="payment-records-card">
          <h3>支付记录</h3>
          <div class="payment-record" v-for="(item, index) in paymentRecords" :key="index">
            <div class="record-info">
              <p><strong>物品名:</strong> {{ item.name }}</p>
              <p><strong>价格:</strong> ￥{{ item.price }}</p>
              <p><strong>支付状态:</strong> 
                <span :class="item.isPaid ? 'status-paid' : 'status-unpaid'">
                  {{ item.isPaid ? '已支付' : '未支付' }}
                </span>
              </p>
            </div>
            <div class="record-action">
              <button v-if="!item.isPaid" @click="goToPayment(item.id)" class="pay-btn">
                去支付
              </button>
              <button v-if="item.isPaid" @click="cancelPayment(item.id)" class="cancel-btn">
                取消支付
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </template>



  <script>

    const BACKEND_BASE_URL = import.meta.env.VITE_API_BACKEND_BASE_URL;
    import axios from 'axios';

  export default {
    data() {
      return {
        user: null, // 存储用户信息
        paymentRecords: [] // 存储支付记录
      };
    },
    created() {
    //   this.fetchUserData();
      this.fetchPaymentRecords();
    },
    methods: {
      // 获取用户信息
    //   async fetchUserData() {
    //     try {
    //       const response = await axios.get('${BACKEND_BASE_URL}/user'); // 假设此接口返回用户信息
    //       const data = await response.json();
    //       this.user = data;
    //     } catch (error) {
    //       console.error('获取用户信息失败:', error);
    //     }
    //   },
      
      // 获取支付记录
      async fetchPaymentRecords() {
        const userId = localStorage.getItem('userId'); // 从 localStorage 获取 userId
        try {
            const response = await axios.get(`${BACKEND_BASE_URL}/transaction/${userId}`);
            this.paymentRecords = response.data; // 假设 API 返回的数据是数组格式
        } catch (error) {
            console.error('获取支付记录失败:', error);
        }
      },
  
      // 跳转到支付页面
      async goToPayment(transactionId) {
        try {
            const response = await axios.post(`${BACKEND_BASE_URL}/transaction/afterpay/${transactionId}`);
            if (response.status === 200) {
            this.$message.success('支付成功');
            this.fetchPaymentRecords(); // 支付完成后刷新支付记录
            } else {
            this.$message.error('支付失败，请重试');
            }
        } catch (error) {
            console.error('支付失败:', error);
            this.$message.error('支付失败，请重试');
        }
      },
        async cancelPayment(transactionId) {
            try {
                const response = await axios.get(`${BACKEND_BASE_URL}/transaction/cancel/${transactionId}`);
                if (response.status === 200) {
                this.$message.success('取消支付成功');
                this.fetchPaymentRecords(); // 刷新支付记录
                } else {
                this.$message.error('取消支付失败，请重试');
                }
            } catch (error) {
                console.error('取消支付失败:', error);
                this.$message.error('取消支付失败，请重试');
            }
        }
    }
  };
  </script>
  
  <style scoped>
  /* 样式保持不变，沿用之前的设计 */
  .payment-container {
    max-width: 800px;
    margin: 20px auto;
    padding: 10px;
  }
  
  .user-info-card, .payment-records-card {
    background-color: #fff;
    border-radius: 10px;
    padding: 20px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    margin-bottom: 20px;
  }
  
  .user-info-card h2, .payment-records-card h3 {
    color: #333;
  }
  
  .payment-record {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 15px;
    margin-bottom: 10px;
    background-color: #f9f9f9;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }
  
  .record-info p {
    margin: 5px 0;
  }
  
  .status-paid {
    color: green;
  }
  
  .status-unpaid {
    color: red;
  }
  
  .pay-btn {
    background-color: #ff5722;
    color: white;
    border: none;
    padding: 10px 20px;
    border-radius: 5px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }
  
  .pay-btn:hover {
    background-color: #e64a19;
  }
  </style>
  