<template>
    <div class="protocol-list" v-if="protocolId == ''">
        <!-- 页面头部 -->
        <div class="page-header">
            <h2>协议管理</h2>
            <el-button type="primary" @click="addProtocol">
                <el-icon>
                    <Plus />
                </el-icon>
                新建协议
            </el-button>
        </div>

        <!-- 搜索栏 -->
        <el-card shadow="never" class="search-card">
            <el-form :inline="true" :model="searchForm" class="search-form">
                <el-form-item label="协议名称">
                    <el-input v-model="searchForm.name" placeholder="输入名称搜索" clearable />
                </el-form-item>
                <el-form-item label="传输类型">
                    <el-select v-model="searchForm.transportType" clearable placeholder="全部">
                        <el-option label="TCP" value="Tcp" />
                        <el-option label="串口" value="Serial" />
                    </el-select>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="fetchList">查询</el-button>
                    <el-button @click="resetSearch">重置</el-button>
                </el-form-item>
            </el-form>
        </el-card>

        <!-- 协议表格 -->
        <el-card shadow="never" class="table-card">
            <el-table :data="protocolList" border stripe v-loading="loading" style="width: 100%">
                <el-table-column prop="protocolName" label="协议名称" min-width="180" />
                <el-table-column prop="transport.type" label="传输类型" width="100" />
                <el-table-column prop="transport.defaultPort" label="默认端口" width="100" />
                <el-table-column label="初始化步骤" min-width="120">
                    <template #default="{ row }">
                        <el-tag v-for="step in row.handshake" :key="step" size="small" style="margin-right: 4px;">
                            {{ step.name }}
                        </el-tag>
                        <span v-if="!row.handshake || row.handshake.length === 0" style="color: #999;">无</span>
                    </template>
                </el-table-column>
                <el-table-column label="操作数量" width="100" align="center">
                    <template #default="{ row }">
                        <el-tag type="info" size="small">{{ Object.keys(row.operations || {}).length }}</el-tag>
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="180" align="center" fixed="right">
                    <template #default="{ row }">
                        <el-button link type="primary" @click="editProtocol(row)">编辑</el-button>
                        <el-button link type="danger" @click="handleDelete(row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>

            <!-- 分页 -->
            <div class="pagination-container">
                <el-pagination v-model:current-page="pagination.page" v-model:page-size="pagination.size"
                    :total="pagination.total" :page-sizes="[10, 20, 50]"
                    layout="total, sizes, prev, pager, next, jumper" @size-change="fetchList"
                    @current-change="fetchList" />
            </div>
        </el-card>
    </div>
    <template v-else>
        <ProtocolEditor :form="protocolForm" :op="protocolId" @back="initEdit" />
    </template>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
// import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import ProtocolEditor from './ProtocolEditor.vue'
import { get } from '../utils/api'

const protocolId = ref("");
const protocolForm = ref({});

// const router = useRouter()
const loading = ref(false)
const protocolList = ref([{
    "protocolName": "S7",
    "transport": {
        "type": "Tcp",
        "defaultPort": 102
    },
    "framing": {
        "type": "LengthField",
        "lengthFieldOffset": 2,
        "lengthFieldLength": 2,
        "lengthIncludesHeader": true,
        "byteOrder": "BigEndian"
    },
    "connection": {
        "responseTimeoutMs": 1000,
        "interFrameDelayMs": 1000
    },
    "handshake": [
        {
            "name": "COTP Connect",
            "requestTemplate": [
                "03 00 00 16", // TPKT 长度 = 22 bytes
                "11 E0 00 00", // COTP Connect Request
                "00 01 00 C0 01", // 源引用 + 参数
                "0A C1 02", // 目的 + 参数
                "01 02", // TPDU 大小
                "C2 02 01 00"
            ],
            "framing": {
                "type": "Fixed",
                "fixedLength": 22
            },
            "validCondition": "resp[5] == 0xD0" // 确认帧标识 0xD0 (Connect Confirm)
        },
        {
            "name": "S7 Setup Communication",
            "requestTemplate": [
                "03 00",
                "00 19", // 总长度 25 bytes
                "02 F0 80", // PDU 类型
                "32 01 00 00", // S7 头部
                "04 00 00 08", // 参数 + 数据长度
                "00 00 F0 00", // 子功能
                "00 01 00 01", // 参数
                "01 E0" // 保留
            ],
            "framing": {
                "type": "Fixed",
                "fixedLength": 27
            },
            "validCondition": "resp[1] == 0x00 && resp[2] == 0x00"
        }
    ],
    "operations": {
        "ReadDB": {
            "requestTemplate": [
                "03 00",
                "{TpktLength:calc:X4}",
                "02 F0 80",
                "32 01 00 00 00 01",
                "00 0E", // 参数长度
                "00 00", // 数据长度
                "04 01", // 读请求
                "12 0A",
                "10 02", // 传送尺寸（数据类型）
                "{ReadLength:X4}", // 读取字节数
                "{DbNumber:X4}", // DB 号
                "84 00", // 区域（DB）
                "{StartByte:X8}" // 起始字节地址（4字节大端，bit偏移在前3bit）
            ],
            "responseParser": {
                "validCondition": "resp[21] == 0xFF",
                "dataStartIndex": 25,
                "dataLengthExpr": "resp[22]",
                "valueType": "ByteArray"
            }
        },
        "WriteDB": {
            "requestTemplate": [
                "{TpktLength:calc:X4}",
                "02 F0 80",
                "32 01 00 00 00 01",
                "00 0E",
                "{DataLength:calc:X4}", // 数据部分长度（字节数 + 4）
                "05 01", // 写请求
                "12 0A",
                "10 02", // 传送尺寸
                "00 { :X4}",
                "00 01",
                "00 {StartByte:X8}",
                "00 00",
                "00 {WriteLength:X4}", // 写入字节数
                "00 04", // 后面数据头
                "00 12", // 传送尺寸
                "{Data:hex}" // 实际数据，用户提供十六进制字符串
            ],
            "responseParser": {
                "validCondition": "resp[21] == 0xFF",
                "dataStartIndex": 0,
                "dataLengthExpr": "0",
                "valueType": "Empty"
            }
        }
    }
}])

const searchForm = reactive({
    name: '',
    transportType: ''
})

const pagination = reactive({
    page: 1,
    size: 10,
    total: 0
})

// 获取协议列表
function fetchList() {
    loading.value = true
    try {
        get("/getProtocolList", {
            name: searchForm.name,
            transportType: searchForm.transportType,
            page: pagination.page,
            size: pagination.size
        }).then(data => {
            protocolList.value = data || []
        });
        // 假设后端返回 { items: [], total: number }
        // pagination.total = data.total || 0
    } catch (e) {
        ElMessage.error('获取协议列表失败')
    } finally {
        loading.value = false
    }
}

// 重置搜索
function resetSearch() {
    searchForm.name = ''
    searchForm.transportType = ''
    pagination.page = 1
    fetchList()
}

// 编辑协议
function editProtocol(row) {
    protocolForm.value = { ...row };
    protocolId.value = "edit";
    //router.push(`/protocols/${row.id}`)   // 假设协议有唯一 id 字段
}
function addProtocol() {
    protocolForm.value = {};
    protocolId.value = "new";
}
// 编辑协议
function initEdit() {
    protocolId.value = "";
    //router.push(`/protocols/${row.id}`)   // 假设协议有唯一 id 字段
}

// 删除协议
function handleDelete(row) {
    ElMessageBox.confirm(`确定要删除协议 "${row.protocolName}" 吗？`, '警告', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(async () => {
        try {
            //await protocolApi.delete(row.id)
            //ElMessage.success('删除成功')
            //fetchList()
        } catch (e) {
            ElMessage.error('删除失败')
        }
    }).catch(() => { })
}

onMounted(() => {
    fetchList()
})
</script>

<style scoped>
.protocol-list {
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

.search-card {
    margin-bottom: 20px;
    border-radius: 8px;
}

.search-form {
    display: flex;
    flex-wrap: wrap;
}

.table-card {
    border-radius: 8px;
}

.pagination-container {
    display: flex;
    justify-content: flex-end;
    margin-top: 16px;
}
</style>