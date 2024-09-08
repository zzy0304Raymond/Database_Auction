  <template>
    <div class="payment-container" v-if="user && paymentRecords.length">
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
          </div>
        </div>
      </div>
    </div>

  </template>
  
  <script>
  export default {
    data() {
      return {
        user: null, // 存储用户信息
        paymentRecords: [] // 存储支付记录
      };
    },
    created() {
      this.fetchUserData();
      this.fetchPaymentRecords();
    },
    methods: {
      // 获取用户信息
      async fetchUserData() {
        try {
          const response = await fetch('/api/user'); // 假设此接口返回用户信息
          const data = await response.json();
          this.user = data;
        } catch (error) {
          console.error('获取用户信息失败:', error);
        }
      },
      
      // 获取支付记录
      async fetchPaymentRecords() {
        try {
          const response = await fetch('/api/payment-records'); // 假设此接口返回支付记录
          const data = await response.json();
          this.paymentRecords = data;
        } catch (error) {
          console.error('获取支付记录失败:', error);
        }
      },
  
      // 跳转到支付页面
      goToPayment(itemId) {
        this.$router.push({
            name: 'Payment',
            params: { userId: userId, itemId: itemId } // 通过 params 传递 userId 和 itemId
        });
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
  