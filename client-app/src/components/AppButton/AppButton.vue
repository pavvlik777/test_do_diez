<template>
  <button
    class="app-button"
    :class="{ enabled: !this.disabled }"
    :disabled="isDisabled"
    @click="onButtonClick"
  >
    <slot />
  </button>
</template>

<script>
export default {
  name: "AppButton",
  props: {
    disabled: {
      type: Boolean,
      default: false,
    },
    color: {
      type: String,
      default: "primary",
    },
  },
  computed: {
    isDisabled() {
      return this.disabled;
    },
    buttonClass() {
      const classes = [`app-button--${this.color}`];

      return classes.join` `;
    },
  },
  methods: {
    onButtonClick() {
      if (!this.disabled) {
        this.$emit("buttonClick");
      }
    },
  },
};
</script>

<style lang="scss" src="./AppButton.scss"></style>