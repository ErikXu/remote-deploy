<template>
  <a-card>
    <a-input placeholder="ip" v-model="ip"/>
    <a-textarea placeholder="command" v-model="command" :rows="4" />
    <a-button type="primary" @click="submit">
      Submit
    </a-button>
    <a-textarea v-model="value" :rows="10" />
  </a-card>
</template>
<script>

import * as signalR from '@microsoft/signalr'
import { execute } from '@/api/command'
import { v4 as uuidv4 } from 'uuid'

const connection = new signalR.HubConnectionBuilder()
  .withUrl(process.env.BASE_API + '/messages')
  .withAutomaticReconnect()
  .build()

connection.start()

export default {
  data () {
    return {
      ip: '',
      value: '',
      command: ''
    }
  },
  methods: {
    submit () {
      var self = this
      var operatorId = uuidv4()

      var form = {
        operatorId: operatorId,
        ip: self.ip,
        command: self.command
      }
      connection.invoke('Subscribe', operatorId).catch(err => console.error(err))
      connection.on('ReceiveMessage', function (message) {
        self.value += message
      })

      return execute(form).then(response => {
      })
    }
  }
}
</script>

<style lang="less" scoped></style>
