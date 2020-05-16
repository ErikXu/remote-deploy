import request from '@/utils/request'

export function getClients () {
  return request({
    url: '/api/clients',
    method: 'get'
  })
}
