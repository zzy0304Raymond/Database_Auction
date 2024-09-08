<template>
  <div class="payment-container">
    <h1>Payment</h1>

    <!-- 显示从商品接口获取的商品信息 -->
    <el-card class="order-summary">
      <h2>Order Summary</h2>
      <p>Item: {{ product.name }}</p> <!-- 显示商品名称 -->
      <p>Description: {{ product.description }}</p> <!-- 显示商品描述 -->
      <p>Price: US ${{ product.price }}</p> <!-- 显示商品价格 -->
      <p>Quantity: {{ order.quantity }}</p> <!-- 显示用户选择的数量 -->
      <p>Total: US ${{ order.totalAmount }}</p> <!-- 计算并显示总金额 -->
    </el-card>

    <el-form :model="paymentForm" label-width="120px">
      <!-- 信用卡支付表单 -->
      <el-form-item label="Cardholder Name">
        <el-input v-model="paymentForm.cardholderName" placeholder="Enter cardholder name"></el-input>
      </el-form-item>
      
      <el-form-item label="Card Number">
        <el-input v-model="paymentForm.cardNumber" placeholder="Enter card number"></el-input>
      </el-form-item>

      <el-form-item label="Expiration Date">
        <el-date-picker
          v-model="paymentForm.expirationDate"
          type="month"
          format="MM/YY"
          placeholder="MM/YY">
        </el-date-picker>
      </el-form-item>

      <el-form-item label="Security Code (CVV)">
        <el-input v-model="paymentForm.cvv" placeholder="Enter CVV" maxlength="3"></el-input>
      </el-form-item>

      <el-form-item>
        <el-button type="primary" @click="processPayment">Pay Now</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
const BACKEND_BASE_URL = import.meta.env.VITE_API_BACKEND_BASE_URL;

export default {
  name: 'Payment',
  data() {
    return {
      order: {
        quantity: 1, // 默认商品数量为1
        totalAmount: 0, // 默认总金额
      },
      product: {
        name: '',
        description: '',
        price: 0,
      },
      paymentForm: {
        cardholderName: '',
        cardNumber: '',
        expirationDate: '',
        cvv: '',
      },
      isProcessing: false, // 防止重复提交
    };
  },
  async created() {
    // 从路由获取商品编号和用户编号
    const itemId = this.$route.params.itemId;
    const userId = this.$route.params.userId;

    // 调用获取商品信息的接口
    await this.fetchProductDetails(itemId);
  },
  methods: {
    async fetchProductDetails(itemId) {
      try {
        const response = await axios.get(`${BACKEND_BASE_URL}/Auction-items/${itemId}`);
        this.product = response.data;

        // 计算总金额（商品价格 * 数量）
        this.order.totalAmount = this.product.price * this.order.quantity;
      } catch (error) {
        console.error('Error fetching product details:', error);
        this.$message.error('Failed to fetch product details.');
      }
    },
    async processPayment() {
      if (this.isProcessing) return;
      this.isProcessing = true;

      try {
        // 准备支付数据
        const paymentData = {
          itemName: this.product.name,
          quantity: this.order.quantity,
          totalAmount: this.order.totalAmount,
          cardholderName: this.paymentForm.cardholderName,
          cardNumber: this.paymentForm.cardNumber,
          expirationDate: this.paymentForm.expirationDate,
          cvv: this.paymentForm.cvv,
        };

        // 发送支付请求
        const response = await axios.post(`${BACKEND_BASE_URL}/payment`, paymentData);

        // 支付成功
        if (response.data.success) {
          this.$message.success('Payment processed successfully!');
          this.$router.push('/order-confirmation');
        } else {
          this.$message.error('Payment failed. Please try again.');
        }

      } catch (error) {
        console.error('Error processing payment:', error);
        this.$message.error('An error occurred during payment. Please try again later.');
      } finally {
        this.isProcessing = false;
        this.paymentForm.cardNumber = ''; // 清除敏感信息
        this.paymentForm.cvv = ''; 
      }
    },
  },
};
</script>

<style scoped>
.payment-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  background-color: #f9f9f9;
  border-radius: 10px;
}

.order-summary {
  margin-bottom: 20px;
}

.el-form-item {
  margin-bottom: 20px;
}
</style>
