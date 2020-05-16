<template>
  <a-layout-header :class="['light', 'global-header']">
    <div :class="['global-header-wide', 'side']">
      <router-link to="/" :class="['logo', 'light']">
        <img width="32" src="/static/img/icon.png" />
        <a-divider type="vertical" />
        <h1>{{ name }}</h1>
      </router-link>
      <router-link to="/client" class="menu-item">
        Client
      </router-link>
       <a-divider type="vertical" />
      <router-link to="/agent" class="menu-item">
        Agent
      </router-link>
    </div>
  </a-layout-header>
</template>

<script>
import IMenu from '../components/menu/menu'

export default {
  name: 'GlobalHeader',
  components: { IMenu },
  computed: {
  },
  data () {
    return {
      name: 'Deploy Platform'
    }
  },
  methods: {
  }
}
</script>

<style lang="less" scoped>
.menu-item {
  color: rgba(0, 0, 0, 0.45);
  transition: color 0.3s;
  text-decoration: none;
  font-size: 16px;
}

.trigger {
  font-size: 20px;
  line-height: 64px;
  padding: 0 24px;
  cursor: pointer;
  transition: color 0.3s;
  &:hover {
    color: #1890ff;
  }
}
.header-item {
  padding: 0 12px;
  display: inline-block;
  height: 100%;
  cursor: pointer;
  vertical-align: middle;
  i {
    font-size: 16px;
    color: rgba(0, 0, 0, 0.65);
  }
}
.global-header {
  padding: 0 12px 0 0;
  -webkit-box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  position: relative;
  &.light {
    background: #fff;
  }
  &.dark {
    background: #001529;
  }
  .global-header-wide {
    &.head {
      max-width: 1400px;
      margin: auto;
    }
    &.side {
    }
    .logo {
      height: 64px;
      line-height: 58px;
      vertical-align: top;
      display: inline-block;
      padding: 0 12px 0 24px;
      cursor: pointer;
      font-size: 20px;
      &.pc {
        padding: 0 12px 0 0;
      }
      img {
        display: inline-block;
        vertical-align: middle;
      }
      h1 {
        display: inline-block;
        font-size: 16px;
      }
      &.dark h1 {
        color: #fff;
      }
    }
    .global-header-menu {
      display: inline-block;
    }
    .global-header-right {
      float: right;
      &.dark {
        color: #fff;
        i {
          color: #fff;
        }
      }
    }
  }
}
</style>
