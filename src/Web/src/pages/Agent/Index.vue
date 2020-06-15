<template>
  <a-card>
    <h2>Agent List</h2>
    <a-upload :file-list="fileList" :remove="remove" :before-upload="beforeUpload" :multiple="false">
      <a-button> <a-icon type="upload" /> Select </a-button>
    </a-upload>
     <a-button
      type="primary"
      :disabled="fileList.length === 0"
      :loading="uploading"
      @click="upload"
    >
      Upload
    </a-button>
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
import { getAgents, getUploadUrl, uploadAgent } from '@/api/agent'
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
      rowKey: 'sessionId',
      uploadUrl: '',
      fileList: [],
      uploading: false
    }
  },
  methods: {
    remove (file) {
      this.fileList = []
    },
    beforeUpload (file) {
      this.fileList = [file]
      return false
    },
    upload () {
      const { fileList } = this
      const formData = new FormData()
      fileList.forEach(file => {
        formData.append('files[]', file)
      })
      this.uploading = true
      return getUploadUrl().then(urlResponse => {
        uploadAgent(urlResponse, formData).then(response => {
          this.uploading = false
        })
      })
    },
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
