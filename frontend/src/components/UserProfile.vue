<template>
  <div class="user-profile" v-if="isAuthenticated"> 
    <!-- 用户资料卡片 -->
    <el-card class="profile-card">
        <div class="profile-info">
          <h1 class="profile-title">User Profile</h1>
          <!-- 编辑资料按钮，点击时打开编辑模式 -->
          <el-button type="primary" @click="isEditing = !isEditing"> <!-- 切换编辑模式 -->
            {{ isEditing ? 'Cancel' : 'Edit Profile' }} <!-- 根据编辑状态改变按钮文字 -->
          </el-button>
        </div>
      <div class="user-info">
        <!-- 骨架屏组件，加载时显示占位符 -->
        <el-skeleton :loading="loading" animated>
          <template #template>
            <el-skeleton-item variant="text"></el-skeleton-item>
            <el-skeleton-item variant="text"></el-skeleton-item>
          </template>
          <template #default>
            <!-- 根据编辑状态显示不同内容 -->
            <div v-if="!isEditing" class="user-display-info">
              <p>Username: {{ user.username }}</p>
              <p>Email: {{ user.email }}</p>
              <p>Phone: {{ user.phone }}</p>
              <p>Address: {{ user.address }}</p>
            </div>
            <el-form  v-else :model="editForm" class="edit-form">
              <el-form-item label="Username" class="form-item">
                <el-input v-model="editForm.username" class="input-field"></el-input>
              </el-form-item>
              <el-form-item label="Email" class="form-item">
                <el-input v-model="editForm.email" class="input-field"></el-input>
              </el-form-item>
              <el-form-item label="Phone" class="form-item">
                <el-input v-model="editForm.phone" class="input-field"></el-input>
              </el-form-item>
              <el-form-item label="Address" class="form-item">
                <el-input v-model="editForm.address" class="input-field"></el-input>
              </el-form-item>
              <div class="save-button-container">
                <el-button type="primary" @click="saveProfile" class="save-button">Save</el-button>
              </div>
            </el-form>
          </template>
        </el-skeleton>
      </div>
    </el-card>

    <!-- 竞价历史卡片 -->
    <el-card class="bidding-history-card">
      <h2>Bidding History</h2>
      <el-skeleton :loading="loading" animated :count="5">
        <template #template>
          <el-skeleton-item variant="text"></el-skeleton-item>
        </template>
        <template #default>
          <el-table :data="bids" style="width: 100%" align="center" header-align="center">

            <!-- Item Name 列 -->
            <el-table-column prop="itemName" label="Item Name" width="180" align="center">
              <template #default="scope">
                {{ scope.row.itemName }}
              </template>
            </el-table-column>

            <!-- Current Highest Bid 列 -->
            <el-table-column prop="currentHighestBid" label="Current Highest Bid" width="180" align="center">
              <template #default="scope">
                {{ scope.row.currentHighestBid }}
              </template>
            </el-table-column>

            <!-- Your Bid Amount 列 -->
            <el-table-column prop="amount" label="Your Bid" width="180" align="center">
              <template #default="scope">
                {{ scope.row.amount }}
              </template>
            </el-table-column>

            <!-- Timestamp 列 -->
            <el-table-column prop="timestamp" label="Bid Time" width="180" align="center">
              <template #default="scope">
                {{ new Date(scope.row.timestamp).toLocaleString() }}
              </template>
            </el-table-column>

          </el-table>
        </template>
      </el-skeleton>
    </el-card>

        <!-- 购物车卡片 -->
        <el-card class="cart-card">
      <h2>Your Cart</h2>
      <el-skeleton :loading="loading" animated>
        <template #default>
          <el-table :data="cartItems" style="width: 100%" align="center" header-align="center">
            <el-table-column prop="itemName" label="Item Name" width="180" align="center">
              <template #default="scope">
                <el-link @click="goToItemDetail(scope.row.itemId)">
                  {{ scope.row.itemName }}
                </el-link>
              </template>
            </el-table-column>
            <el-table-column prop="price" label="Price" width="100" align="center"></el-table-column>
            <el-table-column label="Action" width="150" align="center">
              <template #default="scope">
                <el-button @click="removeFromCart(scope.row)" type="danger" size="small" align="center">Remove</el-button>
              </template>
            </el-table-column>
          </el-table>
        </template>
      </el-skeleton>
    </el-card>

    <!-- <el-card class="pending-payment-card">
      <h2>Pending Payments</h2>
      <el-skeleton :loading="loading" animated>
        <template #default>
          <el-table :data="pendingPayments" style="width: 100%">
            <el-table-column prop="itemName" label="Item Name" width="180"></el-table-column>
            <el-table-column prop="price" label="Price" width="100"></el-table-column>
            <el-table-column label="Action" width="150">
              <template #default="scope">
                <el-button @click="goToPayment(scope.row)" type="primary" size="small">Pay Now</el-button>
              </template>
            </el-table-column>
          </el-table>
        </template>
      </el-skeleton>
    </el-card> -->

    <el-button type="danger" @click="logout">Logout</el-button>
  </div>
</template>

<script>
import axios from 'axios';

const BACKEND_BASE_URL = import.meta.env.VITE_API_BACKEND_BASE_URL;

export default {
  name: 'UserProfile',
  data() {
    return {
      user: {},
      editForm: {},
      bids: [],
      cartItems: [], // 新增购物车数据
      loading: true,
      isAuthenticated: false,
      isSaving: false,
      isEditing: false, // 增加编辑模式状态
    };
  },
  created() {
    const token = localStorage.getItem('authToken');
    if (token) {
      this.isAuthenticated = true;
      this.user = {};  // 清除上次的用户数据
      this.fetchUserData();
      this.fetchBiddingHistory();
      this.fetchCartItems();
      this.fetchPendingPayments(); // 获取待支付商品
    } else {
      this.$router.push({ name: 'Login', query: { redirect: this.$route.fullPath } });
    }
  },
  methods: {
    // 获取用户数据
    async fetchUserData() {
      const userId = localStorage.getItem('userId');
      try {
        const response = await axios.get(`${BACKEND_BASE_URL}/user/users/${userId}`);
        this.user = response.data;
        this.editForm = { ...response.data };
      } catch (error) {
        console.error('Error fetching user data:', error);
        this.$message.error('Failed to load user data');
      } finally {
        this.loading = false;
      }
    },
    // 获取竞价历史
    async fetchBiddingHistory() {
      const userId = localStorage.getItem('userId');
      try {
        const response = await axios.get(`${BACKEND_BASE_URL}/user/users/${userId}/bids`);
        console.log('Bidding History:', response.data); // 打印数据以调试
        this.bids = response.data;  // 更新 bids 数据结构
      } catch (error) {
        console.error('Error fetching bidding history:', error);
        this.$message.error('Failed to load bidding history');
      } finally {
        this.loading = false;
      }
    },
    // 保存编辑后的用户资料
    async saveProfile() {
      if (this.isSaving) return;
      this.isSaving = true;

      const userId = localStorage.getItem('userId');
      try {
        await axios.put(`${BACKEND_BASE_URL}/user/users/${userId}`, this.editForm);
        this.user = { ...this.editForm };
        this.isEditing = false; // 保存成功后退出编辑模式
        this.$message.success('Profile updated successfully');
      } catch (error) {
        console.error('Error updating profile:', error);
        this.$message.error('Failed to update profile');
      } finally {
        this.isSaving = false;
      }
    },
    async fetchCartItems() {
      const userId = localStorage.getItem('userId');
      try {
        const response = await axios.get(`${BACKEND_BASE_URL}/user/users/${userId}/cart`);
        // 将返回的数据简化为只保留 itemName、price 和 itemId
        this.cartItems = response.data.map(item => ({
          itemName: item.itemName,
          price: item.price,
          itemId: item.itemId
        }));
      } catch (error) {
        console.error('Error fetching cart items:', error);
        this.$message.error('Failed to load cart items');
      }
    },
    
    goToItemDetail(itemId) {
      this.$router.push({ name: 'ItemDetail', params: { id: itemId } });
    },
    // 从购物车中移除物品
    async removeFromCart(item) {
      const userId = localStorage.getItem('userId');
      try {
        await axios.delete(`${BACKEND_BASE_URL}/user/users/${userId}/cart/${item.itemId}`);
        this.$message.success('Item removed from cart');
        this.fetchCartItems();
      } catch (error) {
        console.error('Error removing item from cart:', error);
        this.$message.error('Failed to remove item from cart');
      }
    },
    logout() {
      // 清除localStorage中的用户相关数据
      localStorage.removeItem('authToken');
      localStorage.removeItem('userId');
      this.isAuthenticated = false; // 更新身份验证状态

      // 重定向到登录页面
      this.$router.push({ name: 'Login' });
    },

    async fetchPendingPayments() {
      // const userId = localStorage.getItem('userId');
      // try {
      //   const response = await axios.get(`${BACKEND_BASE_URL}/user/users/${userId}/pending-payments`);
      //   this.pendingPayments = response.data.map(item => ({
      //     itemName: item.itemName,
      //     price: item.price,
      //     orderId: item.orderId,
      //   }));
      // } catch (error) {
      //   console.error('Error fetching pending payments:', error);
      //   this.$message.error('Failed to load pending payments');
      // }
    },

    goToPayment(order) {
      this.$router.push({ name: 'Payment', params: { orderId: order.orderId } });
    },
    
  },
};
</script>


<style scoped>
.profile-header {
  display: flex;
  align-items: center;
}

.user-profile {
  padding: 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.profile-card,
.bidding-history-card,
.cart-card {
  width: 60%;
 /* max-width: 800px; /* 设置所有card的最大宽度相同 */
  margin-bottom: 20px; /* 保持卡片之间的间距 */
}

.profile-info,
.profile-card h1,
.bidding-history-card h2,
.cart-card h2 {
  margin-bottom: 20px;
}

.profile-title {
  text-align: center;
  font-size: 24px; /* 增大标题字号 */
}

.user-display-info {
  display: grid;
  grid-template-columns: 1fr 1fr; /* 将信息一行显示两个 */
  gap: 10px; /* 信息之间的间距 */
  font-size: 20px;
}

.el-input {
  width: 90%; /* 控制输入框宽度，缩小编辑框 */
  padding: 5px; /* 调整内边距使输入框变小 */
  font-size: 16px; /* 控制输入框的字体大小 */
  margin: 0 auto; /* 居中 */
}

.edit-form {
  display: grid;
  grid-template-columns: 1fr 1fr; /* 一行两个编辑框 */
  gap: 20px; /* 编辑框之间的间距 */
  align-items: start; /* 使编辑框和标签顶部对齐 */
}

.form-item {
  display: flex;
  flex-direction: column; /* 使表单项保持列布局 */
  width: 100%; /* 确保宽度一致 */
}

.el-form-item label {
  font-size: 16px; /* 增大标签字体 */
  margin-bottom: 5px; /* 调整标签和输入框之间的间距 */
  text-align: left; /* 标签左对齐 */
}

.input-field {
  font-size: 16px; /* 增大输入框内容的字体 */
  padding: 5px; /* 调整输入框的内边距 */
}

.save-button-container {
  grid-column: span 2; /* 跨两列以居中显示 */
  display: flex;
  justify-content: center; /* 保存按钮居中 */
}

.save-button {
  font-size: 14px; /* 调整按钮文字大小 */
  padding: 8px 16px; /* 调整按钮的内边距 */
  max-width: 120px; /* 控制按钮最大宽度 */
}


</style>
