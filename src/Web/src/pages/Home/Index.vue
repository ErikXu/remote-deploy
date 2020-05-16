<template>
  <a-card>
    <a-button type="primary" @click="submit">
      Submit
    </a-button>
    <a-textarea v-model="value" :rows="10" />
  </a-card>
</template>
<script>
import * as signalR from '@microsoft/signalr'

const connection = new signalR.HubConnectionBuilder()
  .withUrl(process.env.BASE_API + '/messages')
  .withAutomaticReconnect()
  .build()

connection.start()

export default {
  data () {
    return {
      value: ''
    }
  },
  methods: {
    submit () {
      var self = this

      connection.invoke('Subscribe', 'Temp').catch(err => console.error(err))
      connection.on('ReceiveMessage', function (message) {
        self.value += message
      })
    }
  }
}
</script>

<style lang="less" scoped></style>
