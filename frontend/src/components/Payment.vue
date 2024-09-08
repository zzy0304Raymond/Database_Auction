<template>
  <div class="payment-container">

    <!-- 支付表单 -->
    <el-form :model="paymentForm" label-width="120px" class="payment-form">
      <el-form-item label="持卡人姓名">
        <el-input v-model="paymentForm.cardholderName" placeholder="请输入持卡人姓名"></el-input>
      </el-form-item>
      <el-form-item label="信用卡号">
        <el-input v-model="paymentForm.cardNumber" placeholder="请输入信用卡号"></el-input>
      </el-form-item>
      <el-form-item label="有效期">
        <el-input v-model="paymentForm.expirationDate" placeholder="MM/YY"></el-input>
      </el-form-item>
      <el-form-item label="CVV">
        <el-input v-model="paymentForm.cvv" placeholder="CVV" type="password"></el-input>
      </el-form-item>

      <!-- 支付按钮 -->
      <el-button type="primary" @click="submitPayment" :loading="isProcessing" :disabled="isProcessing">提交支付</el-button>
    </el-form>

    <!-- 取消支付按钮 -->
    <div class="cancel-section">
      <p class="warning">一经放弃，将无法再支付本次商品。</p>
      <el-button type="danger" @click="cancelPayment" :loading="isCancelling" :disabled="isCancelling">取消支付</el-button>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

const BACKEND_BASE_URL = import.meta.env.VITE_API_BACKEND_BASE_URL;

export default {
  props: ['userId', 'itemId', 'transactionId'], // 接收从路由传递的 userId 和 itemId
  data() {
    return {
      paymentForm: {
        cardholderName: '',
        cardNumber: '',
        expirationDate: '',
        cvv: '',
      },
      isProcessing: false, // 控制支付按钮状态
      isCancelling: false,  // 控制取消支付按钮状态
    };
  },
  methods: {
    // 提交支付
    async submitPayment() {
      if (this.isProcessing) return; // 防止重复提交
      this.isProcessing = true;

      try {
        // 拼接 paymentMethod 字符串，假设这是信用卡信息的格式
        const paymentMethod = `Cardholder: ${this.paymentForm.cardholderName}, Card Number: ${this.paymentForm.cardNumber}, Expiration: ${this.paymentForm.expirationDate}, CVV: ${this.paymentForm.cvv}`;
        console.log('Payment method:', paymentMethod); // 打印发送的支付方法

        const response = await axios.post(`${BACKEND_BASE_URL}/transaction/afterpay/${this.transactionId}`, {
          paymentMethod: paymentMethod // 确保发送的是包含 paymentMethod 字段的对象
        }, {
          headers: {
            'Content-Type': 'application/json' // 使用 JSON 格式传递
          }
        });

        console.log('Payment method:', paymentMethod); // 打印发送的支付方法

        if (response.status === 200) {
          this.$message.success('支付成功！');
          // 处理支付成功后的逻辑
          this.$router.push('/payment-history'); // 跳转到支付历史页面
        } else {
          this.$message.error('支付失败，请重试。');
        }
      } catch (error) {
        console.error('支付请求失败:', error);
        this.$message.error('支付失败，请稍后重试。');
      } finally {
        this.isProcessing = false;
      }
    },


    // 取消支付
    async cancelPayment() {
      if (this.isCancelling) return; // 防止重复操作
      this.isCancelling = true;

      try {
        const response = await axios.get(`${BACKEND_BASE_URL}/transaction/cancel/${this.transactionId}`);
        if (response.status === 200) {
          this.$message.success('支付已取消');
          this.$router.push('/payment-history'); // 跳转到支付历史页面
        } else {
          this.$message.error('取消支付失败，请重试。');
        }
      } catch (error) {
        console.error('取消支付请求失败:', error);
        this.$message.error('取消支付失败，请稍后重试。');
      } finally {
        this.isCancelling = false;
      }
    },
  },
};
</script>

<style scoped>
.payment-container {
  max-width: 600px;
  margin: 50px auto;
  padding: 20px;
  background-color: #fff;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.el-form-item {
  margin-bottom: 20px;
}

.cancel-section {
  margin-top: 20px;
  text-align: center;
}

.warning {
  color: red;
  font-size: 14px;
  margin-bottom: 10px;
}
</style>
