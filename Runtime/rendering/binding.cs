﻿using System;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;

namespace Unity.UIWidgets.rendering {
    public class RendererBinding : PaintingBinding {
        public new static RendererBinding instance {
            get { return (RendererBinding) PaintingBinding.instance; }
            set { PaintingBinding.instance = value; }
        }

        public RendererBinding() {
            this._pipelineOwner = new PipelineOwner(
                onNeedVisualUpdate: this.ensureVisualUpdate
            );

            Window.instance.onMetricsChanged += this.handleMetricsChanged;
            Window.instance.onTextScaleFactorChanged += this.handleTextScaleFactorChanged;
            this.initRenderView();
            D.assert(this.renderView != null);
            this.addPersistentFrameCallback(this._handlePersistentFrameCallback);
        }

        public void initRenderView() {
            D.assert(this.renderView == null);
            this.renderView = new RenderView(configuration: this.createViewConfiguration());
            this.renderView.scheduleInitialFrame();
        }

        public PipelineOwner pipelineOwner {
            get { return this._pipelineOwner; }
        }

        readonly PipelineOwner _pipelineOwner;

        public RenderView renderView {
            get { return (RenderView) this._pipelineOwner.rootNode; }
            set { this._pipelineOwner.rootNode = value; }
        }

        protected virtual void handleMetricsChanged() {
            this.renderView.configuration = this.createViewConfiguration();
            this.scheduleForcedFrame();
        }

        protected virtual void handleTextScaleFactorChanged() {
        }

        protected virtual ViewConfiguration createViewConfiguration() {
            var devicePixelRatio = Window.instance.devicePixelRatio;
            return new ViewConfiguration(
                size: Window.instance.physicalSize / devicePixelRatio,
                devicePixelRatio: devicePixelRatio
            );
        }

        void _handlePersistentFrameCallback(TimeSpan timeStamp) {
            this.drawFrame();
        }

        protected virtual void drawFrame() {
            this.pipelineOwner.flushLayout();
            this.pipelineOwner.flushCompositingBits();
            this.pipelineOwner.flushPaint();
            this.renderView.compositeFrame();
        }

        public override void hitTest(HitTestResult result, Offset position) {
            D.assert(this.renderView != null);
            this.renderView.hitTest(result, position: position);
            base.hitTest(result, position);
        }
    }
}