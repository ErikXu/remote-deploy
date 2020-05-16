<template>
  <a-layout>
    <a-layout>
      <global-header />
      <a-layout-content :style="{minHeight: minHeight, margin: '20px 20px 0'}">
        <slot></slot>
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>

<script>
import GlobalHeader from './GlobalHeader'

const minHeight = window.innerHeight - 64 - 24 - 122

export default {
  name: 'GlobalLayout',
  components: { GlobalHeader },
  data () {
    return {
      minHeight: minHeight + 'px'
    }
  },
  computed: {
  },
  methods: {
  }
}
</script>

<style lang="less" scoped>
  .setting{
    background-color: #1890ff;
    color: #fff;
    border-radius: 5px 0 0 5px;
    line-height: 40px;
    font-size: 22px;
    width: 40px;
    height: 40px;
    box-shadow: -2px 0 8px rgba(0, 0, 0, 0.15);
  }
</style>
