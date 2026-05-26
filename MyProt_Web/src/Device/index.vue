<template>
    <div class="device-tag-manager">
        <div class="page-header">
            <h2>设备与标签管理</h2>
        </div>
        <el-tabs v-model="activeTab" type="border-card" class="manager-tabs">
            <!-- Tab 1: 设备管理 -->
            <el-tab-pane label="设备列表" name="devices">
                <DeviceList />
            </el-tab-pane>

            <!-- Tab 2: 标签管理 -->
            <el-tab-pane label="标签列表" name="tags">
                <TagList />
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script setup>
import { ref } from 'vue'
import DeviceList from './DeviceList.vue'
import TagList from './TagList.vue'

const activeTab = ref('devices')
</script>

<style scoped>
.device-tag-manager {
    padding: 18px;
    margin: 0 auto;
}

.page-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
}
.page-header h2 {
    margin: 0;
    font-size: 22px;
    font-weight: 600;
}

.manager-tabs {
    border-radius: 8px;
    overflow: hidden;
}
</style>