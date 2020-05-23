<template>
  <a-card>
    <a-form :form="form" class="form">
      <a-form-item
        label="IP"
        :label-col="{ span: 2 }"
        :wrapper-col="{ span: 22 }"
        :required="true"
      >
        <a-input
          placeholder="Please input IP"
          v-decorator="[
            'ip',
            {
              rules: [
                { required: true, message: 'Please input IP', whitespace: true }
              ]
            }
          ]"
        />
      </a-form-item>
      <a-form-item
        label="Command"
        :label-col="{ span: 2 }"
        :wrapper-col="{ span: 22 }"
        :required="true"
      >
        <a-textarea
          rows="4"
          placeholder="Please input the command"
          v-decorator="[
            'command',
            {
              rules: [
                {
                  required: true,
                  message: 'Please input the command',
                  whitespace: true
                }
              ]
            }
          ]"
        />
      </a-form-item>
      <a-form-item :wrapper-col="{ span: 22, offset: 2 }">
        <a-button type="primary" html-type="submit" @click="submit">
          Submit
        </a-button>
      </a-form-item>
      <a-form-item :wrapper-col="{ span: 22, offset: 2 }">
        <prism language="bash" :code="output"></prism>
      </a-form-item>
    </a-form>
  </a-card>
</template>
<script>
import * as signalR from '@microsoft/signalr'
import { execute } from '@/api/command'
import { v4 as uuidv4 } from 'uuid'
import Prism from 'vue-prismjs'
import 'prismjs/themes/prism-tomorrow.css'
import 'prismjs/components/prism-markup-templating.js'
import 'prismjs/components/prism-php.js'

const connection = new signalR.HubConnectionBuilder()
  .withUrl(process.env.BASE_API + '/messages')
  .withAutomaticReconnect()
  .build()

connection.start()

export default {
  name: 'Home',
  components: {
    Prism
  },
  mounted () {

  },
  data () {
    return {
      form: this.$form.createForm(this),
      operatorId: '',
      output: 'Command output here...'
    }
  },
  methods: {
    submit () {
      var self = this
      self.output = ''

      self.form.validateFields((err, values) => {
        if (!err) {
          if (self.operatorId !== '') {
            connection
              .invoke('Unsubscribe', self.operatorId)
              .catch(err => console.error(err))
          } else {
            connection.on('ReceiveMessage', function (message) {
              self.output += message
            })
          }

          self.operatorId = uuidv4()
          var form = {
            operatorId: self.operatorId,
            ip: values.ip,
            command: values.command
          }

          connection
            .invoke('Subscribe', self.operatorId)
            .catch(err => console.error(err))

          return execute(form).then(response => {})
        }
      })
    }
  }
}
</script>

<style lang="less" scoped></style>
