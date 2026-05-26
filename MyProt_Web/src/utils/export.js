/**
 * 将 JSON 对象导出为文件并触发浏览器下载
 * @param {Object} data  - 要导出的数据对象
 * @param {string} fileName - 下载文件名（不含扩展名）
 */
export function exportJSON(data, fileName = 'config') {
  const jsonStr = JSON.stringify(data, null, 2)
  const blob = new Blob([jsonStr], { type: 'application/json' })
  const url = URL.createObjectURL(blob)

  const link = document.createElement('a')
  link.href = url
  link.download = `${fileName}.json`
  document.body.appendChild(link)
  link.click()

  document.body.removeChild(link)
  URL.revokeObjectURL(url)
}