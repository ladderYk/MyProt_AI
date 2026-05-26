<template>
    <div class="protocol-editor">
        <!-- 返回按钮 -->
        <el-page-header class="page-header" @back="backFc">
            <template #content>
                <span class="page-title">协议</span>
            </template>
        </el-page-header>

        <el-form :model="form" label-width="160px" class="editor-form">
            <!-- 基本信息卡片 -->
            <el-card shadow="never" class="section-card">
                <template #header>
                    <div class="card-header">
                        <el-icon>
                            <InfoFilled />
                        </el-icon>
                        <span>基本信息</span>
                    </div>
                </template>
                <el-row :gutter="20">
                    <el-col :span="12">
                        <el-form-item label="协议名称">
                            <el-input v-model="form.protocolName" placeholder="如 ModbusTCP" />
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item label="传输类型">
                            <el-select v-model="form.transport.type">
                                <el-option label="TCP" value="Tcp" />
                                <el-option label="串口" value="Serial" />
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-form-item label="默认端口" v-if="form.transport.type == 'Tcp'">
                    <el-input-number v-model="form.transport.defaultPort" :min="1" :max="65535" />
                </el-form-item>
            </el-card>

            <!-- Framing 配置卡片 -->
            <el-card shadow="never" class="section-card">
                <template #header>
                    <div class="card-header">
                        <el-icon>
                            <Connection />
                        </el-icon>
                        <span>帧解析配置</span>
                    </div>
                </template>
                <el-form-item label="帧类型">
                    <el-select v-model="form.framing.type">
                        <el-option label="长度字段" value="LengthField" />
                        <el-option label="固定长度" value="Fixed" />
                        <el-option label="无 (需外部指定)" value="Raw" />
                    </el-select>
                </el-form-item>
                <template v-if="form.framing.type === 'LengthField'">
                    <el-form-item label="长度字段偏移">
                        <el-input-number v-model="form.framing.lengthFieldOffset" :min="0" />
                    </el-form-item>
                    <el-form-item label="长度字段字节数">
                        <el-input-number v-model="form.framing.lengthFieldLength" :min="1" :max="4" />
                    </el-form-item>
                    <el-form-item label="长度包含帧头">
                        <el-switch v-model="form.framing.lengthIncludesHeader" />
                    </el-form-item>
                    <el-form-item label="字节序">
                        <el-select v-model="form.framing.byteOrder">
                            <el-option label="大端" value="BigEndian" />
                            <el-option label="小端" value="LittleEndian" />
                        </el-select>
                    </el-form-item>
                    <el-form-item v-if="!form.framing.lengthIncludesHeader" label="帧头固定长度">
                        <el-input-number v-model="form.framing.headerLength" :min="1" />
                    </el-form-item>
                </template>
                <template v-if="form.framing.type === 'Fixed'">
                    <el-form-item label="固定长度">
                        <el-input-number v-model="form.framing.fixedLength" :min="1" />
                    </el-form-item>
                </template> </el-card>

            <!-- 操作配置卡片 -->
            <!-- 操作定义卡片 -->
            <el-card shadow="never" class="section-card">
                <template #header>
                    <div class="card-header">
                        <el-icon>
                            <Operation />
                        </el-icon>
                        <span>操作定义</span>
                        <el-button type="primary" size="small" @click="addOperation"
                            style="float:right;">新增操作</el-button>
                    </div>
                </template>

                <div class="operation-layout">
                    <!-- 左侧操作列表 -->
                    <div class="op-list">
                        <el-menu :default-active="activeOpName" @select="onOpSelect">
                            <el-menu-item v-for="(op, name) in form.operations" :key="name" :index="name">
                                <span class="op-name">{{ name }}</span>
                                <el-tag v-if="op.isKeepAlive" type="success" size="small" class="op-tag">心跳</el-tag>
                            </el-menu-item>
                        </el-menu>
                        <el-empty v-if="Object.keys(form.operations).length === 0" description="暂无操作" />
                    </div>

                    <!-- 右侧编辑区域 -->
                    <div class="op-editor">
                        <!-- 右侧编辑区域中，操作名称编辑 -->
                        <template v-if="activeOpName && form.operations[activeOpName]">
                            <!-- 操作名称编辑行 -->
                            <div class="op-name-editor">
                                <el-input v-model="editingOpName" placeholder="操作名称" @blur="renameOp"
                                    @keyup.enter="renameOp" style="width: 300px; margin-bottom: 16px;">
                                    <template #prepend>操作名</template>
                                </el-input>
                                <el-button v-if="editingOpName !== activeOpName" type="warning" size="small"
                                    @click="renameOp" style="margin-left: 8px;">
                                    应用改名
                                </el-button>
                            </div>

                            <OperationEditor :op="form.operations[activeOpName]" />

                            <div class="op-editor-actions">
                                <el-button type="danger" @click="deleteOperation(activeOpName)">删除此操作</el-button>
                            </div>
                        </template>
                        <el-empty v-else description="请从左侧选择操作" />
                    </div>
                </div>
            </el-card>

            <div class="submit-area">
                <el-button type="primary" size="large" @click="save">保存协议</el-button>
                <el-button type="success" size="large" @click="exportConfig">导出 JSON 文件</el-button>
                <el-button size="large" @click="backFc">取消</el-button>
            </div>
        </el-form>
    </div>
</template>

<script setup>
import { reactive, ref, computed, watch } from 'vue'
// import { useRoute, useRouter } from 'vue-router'
// import { protocolApi } from '@/api'
import { ElMessage } from 'element-plus'
import OperationEditor from './OperationEditor.vue'
import { exportJSON } from '@/utils/export'

const emits = defineEmits(["back"]);
const props = defineProps(['form'])

const backFc = () => emits("back");
// 当前编辑的名称（与 activeOpName 同步）
const editingOpName = ref('')

// const route = useRoute()
// const router = useRouter()
// const protocolId = route.params.id

const form = reactive({
    protocolName: '',
    transport: { type: 'Tcp', defaultPort: 102 },
    framing: {
        type: 'LengthField',
        lengthFieldOffset: 2,
        lengthFieldLength: 2,
        lengthIncludesHeader: true,
        byteOrder: 'BigEndian',
        headerLength: null
    },
    operations: {}  // key -> operation object
})
if (props.form.protocolName != undefined) {
    form.protocolName = props.form.protocolName;
    form.transport = props.form.transport;
    form.framing = props.form.framing;
    form.operations = props.form.operations;
}
const activeOpNames = ref('')
const selectedInitOp = ref('')

const operationNames = computed(() => Object.keys(form.operations))

// 加载已有协议
async function loadProtocol() {
    // if (protocolId !== 'new') {
    //     const { data } = await protocolApi.get(protocolId)
    //     Object.assign(form, data)
    // }
}
loadProtocol()

function addToInitSeq(opName) {
    if (opName && !form.initSequence.includes(opName)) {
        form.initSequence.push(opName)
    }
    selectedInitOp.value = ''
}
function removeInit(index) {
    form.initSequence.splice(index, 1)
}

async function save() {
    const payload = { ...form }
    if (protocolId === 'new') {
        // await protocolApi.create(payload)
    } else {
        // await protocolApi.update(protocolId, payload)
    }
    ElMessage.success('保存成功')
    //router.push('/protocols')
}
// 当前选中的操作名（用于高亮和右侧编辑）
const activeOpName = ref('')

// 选择操作
function onOpSelect(name) {
    activeOpName.value = name
}

// 新增操作时，自动选中新操作
function addOperation() {
    let newName = `NewOp_${Date.now()}`
    while (form.operations[newName]) newName += '_1'
    form.operations[newName] = {
        requestTemplate: [''],
        expectedResponseLength: null,
        validCondition: '',
        isKeepAlive: false
    }
    activeOpName.value = newName
}

// 删除操作后，若删除的是当前选中项，则清空选中
function deleteOperation(name) {
    delete form.operations[name]
    if (activeOpName.value === name) {
        activeOpName.value = ''
    }
    // 清理 initSequence 等 (保持原逻辑)
}

// 监听选中操作变化，同步编辑框
watch(activeOpName, (newName) => {
    editingOpName.value = newName || ''
})

/**
 * 重命名当前操作
 */
function renameOp() {
    const oldName = activeOpName.value
    const newName = editingOpName.value?.trim()
    if (!oldName || !newName || oldName === newName) return

    // 重名检查
    if (form.operations[newName]) {
        ElMessage.error(`操作名称 "${newName}" 已存在，请更换`)
        editingOpName.value = oldName
        return
    }

    // 1. 替换 operations 键
    form.operations[newName] = form.operations[oldName]
    delete form.operations[oldName]

    // 2. 更新初始化序列中的引用
    const idx = form.initSequence.indexOf(oldName)
    if (idx !== -1) form.initSequence[idx] = newName

    // 3. 更新保活操作引用
    if (form.keepAlive.operation === oldName) {
        form.keepAlive.operation = newName
    }

    // 4. 切换选中状态
    activeOpName.value = newName
    ElMessage.success('操作已重命名')
}
function exportConfig() {
    const fileName = form.protocolName || 'protocol'
    exportJSON(form, fileName)
    ElMessage.success('配置文件已导出')
}
</script>

<style scoped>
.protocol-editor {
    padding: 18px;
    margin: 0 auto;
}

.page-header {
    margin-bottom: 20px;
}

.editor-form {
    background: transparent;
}

.section-card {
    margin-bottom: 20px;
    border-radius: 8px;
    border: 1px solid #ebeef5;
    transition: box-shadow 0.3s;
}

.section-card:hover {
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
}

.card-header {
    display: flex;
    align-items: center;
    gap: 8px;
    font-weight: 600;
    font-size: 16px;
}

.op-title {
    display: flex;
    align-items: center;
    gap: 10px;
}

.submit-area {
    text-align: center;
    margin-top: 30px;
}

.operation-layout {
    display: flex;
    gap: 20px;
    min-height: 400px;
}

.op-list {
    width: 220px;
    flex-shrink: 0;
    border-right: 1px solid #ebeef5;
    padding-right: 10px;
}

.op-list .el-menu {
    border-right: none;
}

.op-name {
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
}

.op-tag {
    margin-left: 8px;
}

.op-editor {
    flex: 1;
    padding-left: 10px;
}

.op-editor-actions {
    margin-top: 20px;
    text-align: right;
}

/* 响应式：小屏时上下堆叠 */
@media (max-width: 768px) {
    .operation-layout {
        flex-direction: column;
    }

    .op-list {
        width: 100%;
        border-right: none;
        border-bottom: 1px solid #ebeef5;
        padding-bottom: 10px;
    }
}
</style>