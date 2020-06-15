import request from '@/utils/request'

export function getAgents () {
  return request({
    url: '/api/agents',
    method: 'get'
  })
}

export function getUploadUrl () {
  return request({
    url: '/api/agents/upload/url',
    method: 'get'
  })
}

export function uploadAgent (url, file) {
  return request({
    url: url,
    method: 'put',
    data: file
  })
}