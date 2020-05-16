<template>
  <a-card>
    <h2>Client List</h2>
    <a-table
      :columns="columns"
      :rowKey="rowKey"
      :dataSource="data"
      :loading="loading"
      :pagination="false"
    >
    <template slot="clientType" slot-scope="clientType">
      <a-tag v-if="clientType === 0" color="#f50">Other</a-tag>
      <a-tag v-else-if="clientType === 1" color="#108ee9">Web</a-tag>
      <a-tag v-else-if="clientType === 2" color="#87d068">Agent</a-tag>
    </template>
      <template slot="connectTime" slot-scope="connectTime">{{
        simpleFormat(connectTime)
      }}</template>
    </a-table>
  </a-card>
</template>
<script>
import { getClients } from '@/api/client'
import { simpleFormat } from '@/utils/date'

const columns = [
  {
    title: 'Session',
    key: 'sessionId',
    dataIndex: 'sessionId'
  },
  {
    title: 'Ip',
    dataIndex: 'ip'
  },
  {
    title: 'Port',
    dataIndex: 'port'
  },
  {
    title: 'Client Type',
    dataIndex: 'clientType',
    align: 'center',
    scopedSlots: { customRender: 'clientType' }
  },
  {
    title: 'Connect Time',
    dataIndex: 'connectTime',
    scopedSlots: { customRender: 'connectTime' }
  }
]

export default {
  mounted () {
    this.loading = true
    return getClients().then(response => {
      this.data = response
      this.loading = false
    })
  },
  data () {
    return {
      data: [],
      loading: false,
      columns,
      rowKey: 'sessionId'
    }
  },
  methods: {
    simpleFormat (date) {
      return simpleFormat(date)
    }
  }
}
</script>
<style scoped>
.method {
  text-transform: uppercase;
  text-decoration: none;
  color: white;
  font-size: 0.7em;
  text-align: center;
  padding: 7px;
  margin-right: 5px;
  -moz-border-radius: 2px;
  -webkit-border-radius: 2px;
  -o-border-radius: 2px;
  -ms-border-radius: 2px;
  -khtml-border-radius: 2px;
  border-radius: 2px;
}

.get {
  background-color: #5eaffe;
}
.post {
  background-color: #49cc90;
}
.put {
  background-color: #fca130;
}
.delete {
  background-color: #f93e3e;
}
</style>
