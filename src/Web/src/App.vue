<template>
  <div id="app">
    <router-view/>
  </div>
</template>

<script>

export default {
  name: 'App',
  created () {
  }
}
</script>

<style lang="less">
  #app{
    display: flex;
    height: 100%;
  }
  :global{
    .dragable-ghost{
      border: 1px dashed #aaaaaa;
      opacity: 0.65;
    }
    .dragable-chose{
      border: 1px dashed #aaaaaa;
      opacity: 0.65;
    }
    .dragable-drag{
      border: 1px dashed #aaaaaa;
      opacity: 0.65;
    }

    .page-toggle-enter-active{
      transition: all 0.2s ease-in 0.25s;
    }
    .page-toggle-leave-active{
      transition: all 0.2s ease-out 0s;
    }
    .page-toggle-enter, .page-toggle-leave-to{
      opacity: 0;
      padding: 0px;
    }
  }
</style>
