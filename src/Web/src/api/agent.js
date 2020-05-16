import request from '@/utils/request'

export function getAgents () {
  return request({
    url: '/api/agents',
    method: 'get'
  })
}
