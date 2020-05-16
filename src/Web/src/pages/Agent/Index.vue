<template>
  <a-card>
    <h2>Agent List</h2>
    <a-table
      :columns="columns"
       :rowKey="rowKey"
      :dataSource="data"
      :loading="loading"
      :pagination="false"
    >
      <template slot="connectTime" slot-scope="connectTime">{{
        simpleFormat(connectTime)
      }}</template>
    </a-table>
  </a-card>
</template>
<script>
import { getAgents } from '@/api/agent'
import { simpleFormat } from '@/utils/date'

const columns = [
  {
    title: 'Session',
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
    title: 'Connect Time',
    dataIndex: 'connectTime',
    scopedSlots: { customRender: 'connectTime' }
  }
]

export default {
  mounted () {
    this.loading = true
    return getAgents().then(response => {
      this.data = response
      this.loading = false
    })
  },
  data () {
    return {
      data: [],
      pagination: {},
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
