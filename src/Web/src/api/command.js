import request from '@/utils/request'

export function execute (form) {
  return request.post('api/commands', form)
}
