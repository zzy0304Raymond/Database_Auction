<template>
    <div class="payment-container" v-if="user && transactions.length">
      <!-- 用户信息部分 -->
      <div class="user-info-card">
        <h2>当前用户信息</h2>
        <p><strong>用户名:</strong> {{ user.userName }}</p>
      </div>
  
      <!-- 拍卖记录表格 -->
      <div class="transaction-records-card">
        <h3>拍卖记录</h3>
        <el-table :data="transactions" style="width: 100%">
          <!-- 物品名称 -->
          <el-table-column prop="itemName" label="物品名" align="center">
            <template #default="scope">
              <span>{{ scope.row.itemName }}</span>
            </template>
          </el-table-column>
  
          <!-- 价格 -->
          <el-table-column prop="price" label="价格" align="center">
            <template #default="scope">
              <span>￥{{ scope.row.price }}</span>
            </template>
          </el-table-column>
  
          <!-- 支付状态 -->
          <el-table-column prop="status" label="支付状态" align="center">
            <template #default="scope">
              <span>
                <!-- 根据 status 的值显示不同的状态 -->
                <span v-if="scope.row.status == '0'">未支付</span>
                <span v-else-if="scope.row.status == '1'">已支付</span>
                <span v-else>取消支付</span>
              </span>
            </template>
          </el-table-column>
  
          <!-- 操作 -->
          <el-table-column label="操作" align="center">
            <template #default="scope">
              <!-- 如果未支付，显示去支付按钮 -->
              <el-button v-if="scope.row.status == '0'" type="primary" @click="goToPayment(scope.row.itemID, scope.row.transactionID)">
                去支付
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  
    <div v-else>
      <p>当前没有拍卖记录</p>
    </div>
  </template>
  

  <script>
  import axios from 'axios';
  const BACKEND_BASE_URL = import.meta.env.VITE_API_BACKEND_BASE_URL;
  
  export default {
    data() {
      return {
        user: null, // 存储用户信息
        transactions: [] // 存储拍卖记录
      };
    },
    created() {
      this.fetchTransactionRecords(); // 获取拍卖记录
    },
    methods: {
    // 获取拍卖记录
    async fetchTransactionRecords() {
        const userId = localStorage.getItem('userId'); // 从 localStorage 获取 userId
        try {
        const response = await axios.get(`${BACKEND_BASE_URL}/transaction/${userId}`);
        this.transactions = response.data; // 假设 API 返回的是数组格式
        console.log("DDDDDDDD", this.transactions);
        this.user = { userName: response.data[0].userName }; // 从记录中提取用户名
        } catch (error) {
        console.error('获取拍卖记录失败:', error);
        }
    },

    // 跳转到支付页面
    goToPayment(itemId, transactionId) {
        const userId = localStorage.getItem('userId'); // 获取当前用户的ID

        if (itemId) { // 确保 itemId 存在
        this.$router.push({
            name: 'Payment',
            params: { userId, itemId, transactionId} // 路由参数传递 userId 和 itemId
        });
        } else {
        console.error('itemId is not defined');
        }
    }
    }

  };
  </script>
  
  <style scoped>
  .payment-container {
    max-width: 800px;
    margin: 20px auto;
    padding: 20px;
    background-color: #f7f7f7;
    border-radius: 10px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }
  
  .user-info-card, .transaction-records-card {
    background-color: #fff;
    border-radius: 10px;
    padding: 20px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    margin-bottom: 20px;
  }
  
  .user-info-card h2, .transaction-records-card h3 {
    color: #333;
  }
  
  .table-header, .table-row {
    display: flex;
    justify-content: space-between;
    padding: 10px;
    border-bottom: 1px solid #eaeaea;
  }
  
  .table-header {
    font-weight: bold;
    background-color: #fafafa;
  }
  
  .table-column {
    flex: 1;
    text-align: center;
  }
  
  .pay-btn {
    background-color: #409eff;
    color: white;
    border-radius: 5px;
    padding: 5px 10px;
    border: none;
    cursor: pointer;
  }
  
  .pay-btn:hover {
    background-color: #66b1ff;
  }
  </style>