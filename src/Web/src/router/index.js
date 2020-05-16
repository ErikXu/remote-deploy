import Vue from 'vue'
import Router from 'vue-router'
import PageView from '@/layouts/PageView'

Vue.use(Router)

export const fixedRoutes = [
  {
    path: '/404',
    component: () => import('@/pages/NotFound'),
    hidden: true
  },
  {
    path: '',
    redirect: '/home',
    hidden: true
  },
  {
    path: '/home',
    component: PageView,
    children: [
      {
        path: '',
        name: 'home-index',
        component: () => import('@/pages/Home/Index')
      }
    ]
  },
  {
    path: '/agent',
    component: PageView,
    children: [
      {
        path: '',
        name: 'agent-index',
        component: () => import('@/pages/Agent/Index')
      }
    ]
  },
  {
    path: '/client',
    component: PageView,
    children: [
      {
        path: '',
        name: 'client-index',
        component: () => import('@/pages/Client/Index')
      }
    ]
  }
]

export const dynamicRoutes = []

export default new Router({
  scrollBehavior: () => ({ y: 0 }),
  routes: [
    ...fixedRoutes,
    ...dynamicRoutes
  ]
})
