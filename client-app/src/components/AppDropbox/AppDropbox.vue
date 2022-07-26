<template>
  <div class="app-dropbox">
    <input
      ref="fileInput"
      :name="uploadFileName"
      :disabled="isLoading"
      :accept="supportedFormats"
      type="file"
      class="app-dropbox__input-file"
      @change="fileChanged"
    />
    <p class="app-dropbox__message">
      <template v-if="!isFileAvailable" path="to_upload_file" tag="span">
        <span class="app-dropbox__link">Click here</span>
      </template>
      <template v-else>
        {{ textValue }}
      </template>
    </p>
    <button
      v-if="isFileAvailable"
      class="app-dropbox__remove-button"
      @click.stop="onClickRemove"
    >
      <i class="ts-ico-close" />
    </button>
  </div>
</template>

<script>
const loadingStatuses = { Initial: 0, Loading: 1, Success: 2, Failure: 3 };
const defaultValueModel = { fileName: "", data: null };

const isEmptyValueModel = (model) => !(model.fileName || model.data);

export default {
  name: "AppDropbox",
  props: {
    value: {
      type: Object,
      default: () => ({ ...defaultValueModel }),
    },
    formats: {
      type: Array,
    },
  },
  data() {
    return {
      currentStatus: loadingStatuses.Initial,
      uploadFileName: "",
    };
  },
  computed: {
    supportedFormats() {
      return this.formats && this.formats.length ? this.formats.join(",") : "";
    },
    isFileAvailable() {
      return !!(this.uploadFileName && this.uploadFileName.length);
    },
    isInitial() {
      return this.currentStatus === loadingStatuses.Initial;
    },
    isLoading() {
      return this.currentStatus === loadingStatuses.Loading;
    },
    isSuccess() {
      return this.currentStatus === loadingStatuses.Success;
    },
    isFailure() {
      return this.currentStatus === loadingStatuses.Failure;
    },
    textValue() {
      return this.isLoading ? "Uploading this.uploadFileName" : this.uploadFileName;
    },
  },
  watch: {
    value: {
      handler: function (newValue) {
        if (newValue) {
          this.uploadFileName = newValue.fileName;
        }

        if (this.$refs.fileInput && isEmptyValueModel(newValue)) {
          this.$refs.fileInput.value = ""; // Vue doesn't support v-model for files so it is alternative way to reset input value
          this.currentStatus = loadingStatuses.Initial;
        }
      },
      immediate: true,
    },
  },
  methods: {
    onClickRemove() {
      this.clearValue();
    },
    onFileLoad() {
      this.currentStatus = loadingStatuses.Success;
      this.$emit("input", {
        fileName: this.uploadFileName,
        data: this.$refs.fileInput.files[0],
      });
    },
    fileChanged(e) {
      if (!e.target.files.length) {
        this.currentStatus = loadingStatuses.Initial;
        this.clearValue();

        return;
      }

      const formData = e.target.files[0];
      this.uploadFileName = formData.name;
      this.currentStatus = loadingStatuses.Loading;
      const reader = new FileReader();
      reader.onload = this.onFileLoad;
      reader.readAsDataURL(formData);
    },
    clearValue() {
      this.$refs.fileInput.value = "";
      this.$emit("input", { ...defaultValueModel });
    },
  },
};
</script>

<style lang="scss" src="./AppDropbox.scss"></style>
