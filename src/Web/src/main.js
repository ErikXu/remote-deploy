import Vue from 'vue'
import App from './App'
import router from './router/'
import 'ant-design-vue/dist/antd.css'
import Antd from 'ant-design-vue'
import axios from 'axios'

Vue.prototype.http = axios
Vue.config.productionTip = false
Vue.use(Antd)

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
