<template>
  <div class="device-list-container">
    <div class="toolbar">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="设备ID">
          <el-input v-model="searchForm.id" placeholder="搜索ID" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchDevices">查询</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
      <el-button type="primary" @click="openDeviceDialog()">
        <el-icon><Plus /></el-icon>
        新增设备
      </el-button>
    </div>

    <el-table :data="devices" border stripe v-loading="loading">
      <el-table-column prop="id" label="设备ID" width="150" />
      <el-table-column prop="protocol" label="协议" width="180" />
      <el-table-column prop="host" label="主机/IP" min-width="160" />
      <el-table-column prop="port" label="端口" width="100" />
      <el-table-column label="操作" width="180" align="center">
        <template #default="{ row }">
          <el-button link type="primary" @click="openDeviceDialog(row)">编辑</el-button>
          <el-button link type="danger" @click="deleteDevice(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 新增/编辑设备对话框 -->
    <el-dialog
      v-model="deviceDialogVisible"
      :title="editingDevice ? '编辑设备' : '新增设备'"
      width="500px"
      destroy-on-close
    >
      <el-form :model="deviceForm" label-width="100px">
        <el-form-item label="设备ID">
          <el-input v-model="deviceForm.id" :disabled="!!editingDevice" />
        </el-form-item>
        <el-form-item label="协议">
          <el-select v-model="deviceForm.protocol" placeholder="选择协议">
            <!-- 协议选项可从后端加载 -->
            <el-option label="ModbusTCP" value="ModbusTCP" />
            <el-option label="S7-300" value="S7-300" />
          </el-select>
        </el-form-item>
        <el-form-item label="主机">
          <el-input v-model="deviceForm.host" />
        </el-form-item>
        <el-form-item label="端口">
          <el-input-number v-model="deviceForm.port" :min="1" :max="65535" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="deviceDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveDevice">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'

const loading = ref(false)
const devices = ref([])
const searchForm = reactive({ id: '' })

const deviceDialogVisible = ref(false)
const editingDevice = ref(null)
const deviceForm = reactive({
  id: '',
  protocol: 'ModbusTCP',
  host: '',
  port: 502
})

// 获取设备列表
async function fetchDevices() {
  loading.value = true
  try {
  } catch {
    ElMessage.error('加载设备列表失败')
  } finally {
    loading.value = false
  }
}

// 重置搜索
function resetSearch() {
  searchForm.id = ''
  fetchDevices()
}

// 打开新增/编辑对话框
function openDeviceDialog(row) {
  editingDevice.value = row || null
  if (row) {
    Object.assign(deviceForm, row)
  } else {
    deviceForm.id = ''
    deviceForm.protocol = 'ModbusTCP'
    deviceForm.host = ''
    deviceForm.port = 502
  }
  deviceDialogVisible.value = true
}

// 保存设备
async function saveDevice() {
  if (!deviceForm.id) {
    ElMessage.warning('设备ID不能为空')
    return
  }
  try {
    if (editingDevice.value) {
      ElMessage.success('设备更新成功')
    } else {
      ElMessage.success('设备创建成功')
    }
    deviceDialogVisible.value = false
    fetchDevices()
  } catch {
    ElMessage.error('保存失败')
  }
}

// 删除设备
function deleteDevice(row) {
  ElMessageBox.confirm(`确定要删除设备 "${row.id}" 吗？`, '警告', { type: 'warning' })
    .then(async () => {
      ElMessage.success('已删除')
      fetchDevices()
    })
    .catch(() => {})
}

onMounted(fetchDevices)
</script>

<style scoped>
.device-list-container {
  padding: 10px 0;
}
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 16px;
  flex-wrap: wrap;
}
</style>