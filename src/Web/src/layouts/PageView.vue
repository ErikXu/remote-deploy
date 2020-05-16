<template>
  <global-layout>
    <transition name="page-toggle">
      <router-view />
    </transition>
  </global-layout>
</template>

<script>
import GlobalLayout from './GlobalLayout'
export default {
  name: 'MenuView',
  components: {GlobalLayout},
  computed: {
  },
  created () {
  },
  methods: {
  }
}
</script>

<style scoped>

</style>
