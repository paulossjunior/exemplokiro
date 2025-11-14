<script setup lang="ts">
import { RouterView } from 'vue-router'
import OfflineIndicator from '@/components/common/OfflineIndicator.vue'
</script>

<template>
  <div>
    <OfflineIndicator />
    <RouterView />
  </div>
</template>
